using AutoMapper;
using BERihalCodestackerChallenge2025.Data;
using BERihalCodestackerChallenge2025.DTOs;
using BERihalCodestackerChallenge2025.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BERihalCodestackerChallenge2025.Controllers
{
    [ApiController]
    [Route("")]
    [Authorize] //  Admin, Investigator, Officer (roles checked per-action)
    public class CasesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public CasesController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // ================================================================
        // POST: api/cases
        // Description: Create a new case and link to existing crime reports
        // ================================================================
        [HttpPost("CreateCase")]
        [Authorize(Roles = "Admin,Investigator")]
        public async Task<IActionResult> CreateCase([FromBody] CaseCreateDto dto)
        {
            //  Map DTO → Model
            var newCase = _mapper.Map<Case>(dto);
            newCase.CreatedAt = DateTime.UtcNow;

            // Get user who created this case (from token)
            var username = User.Identity?.Name;
            var creator = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (creator == null)
                return Unauthorized("Invalid user.");

            newCase.CreatedByUserId = creator.UserId;
            newCase.CaseNumber = $"CASE-{DateTime.UtcNow:yyyy}-{Guid.NewGuid().ToString().Substring(0, 6).ToUpper()}";

            _context.Cases.Add(newCase);
            await _context.SaveChangesAsync();

            //  Link crime reports if provided
            if (dto.ReportIds != null && dto.ReportIds.Any())
            {
                foreach (var reportId in dto.ReportIds)
                {
                    if (await _context.CrimeReports.AnyAsync(r => r.Id == reportId))
                    {
                        _context.CaseReports.Add(new CaseReport
                        {
                            CaseId = newCase.CaseId,
                            ReportId = reportId,
                            LinkedAt = DateTime.UtcNow
                        });
                    }
                }
                await _context.SaveChangesAsync();
            }

            return CreatedAtAction(nameof(GetCaseById), new { id = newCase.CaseId },
                new { newCase.CaseNumber, newCase.Name, newCase.AuthorizationLevel });
        }

        // ================================================================
        // PUT: api/cases/{id}
        // Description: Update an existing case (Admin or Investigator)
        // ================================================================
        [HttpPut("UpdateCase/{id:int}")]
        [Authorize(Roles = "Admin,Investigator")]
        public async Task<IActionResult> UpdateCase(int id, [FromBody] CaseUpdateDto dto)
        {
            var existingCase = await _context.Cases.FindAsync(id);
            if (existingCase == null)
                return NotFound("Case not found.");

            _mapper.Map(dto, existingCase);
            existingCase.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ================================================================
        // GET: api/cases
        // Description: Return list of all cases with 100-char trimmed description
        // ================================================================
        [HttpGet("GetAllCases")]
        [Authorize(Roles = "Admin,Investigator,Officer")]
        public async Task<ActionResult<IEnumerable<CaseListItemDto>>> GetAllCases([FromQuery] string? search)
        {
            var query = _context.Cases
                .Include(c => c.CreatedByUser)
                .AsQueryable();

            //  Search by name or description
            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(c => c.Name.Contains(search) || c.Description.Contains(search));

            var cases = await query.OrderByDescending(c => c.CreatedAt).ToListAsync();

            var result = _mapper.Map<IEnumerable<CaseListItemDto>>(cases);
            return Ok(result);
        }

        // ================================================================
        // GET: api/cases/{id}
        // Description: Case details including counts of related entities
        // ================================================================
        [HttpGet("GetCaseById/{id:int}")]
        [Authorize(Roles = "Admin,Investigator,Officer")]
        public async Task<IActionResult> GetCaseById(int id)
        {
            var caseEntity = await _context.Cases
                .Include(c => c.CreatedByUser)
                .Include(c => c.Assignees)
                .Include(c => c.Evidences)
                .Include(c => c.CaseParticipants)
                .ThenInclude(cp => cp.Participant)
                .FirstOrDefaultAsync(c => c.CaseId == id);

            if (caseEntity == null)
                return NotFound("Case not found.");

            var dto = _mapper.Map<CaseDetailsDto>(caseEntity);

            // Counts
            dto.NumberOfAssignees = caseEntity.Assignees?.Count ?? 0;
            dto.NumberOfEvidences = caseEntity.Evidences?.Count(e => !e.IsSoftDeleted) ?? 0;
            dto.NumberOfSuspects = caseEntity.CaseParticipants?.Count(p => p.Role == ParticipantRole.Suspect) ?? 0;
            dto.NumberOfVictims = caseEntity.CaseParticipants?.Count(p => p.Role == ParticipantRole.Victim) ?? 0;
            dto.NumberOfWitnesses = caseEntity.CaseParticipants?.Count(p => p.Role == ParticipantRole.Witness) ?? 0;


            return Ok(dto);
        }

        //// ================================================================
        //// GET: api/cases/{id}/assignees
        //// Description: List all assigned officers/investigators
        //// ================================================================
        //[HttpGet("{id:int}/assignees")]
        //[Authorize(Roles = "Admin,Investigator,Officer")]
        //public async Task<IActionResult> GetCaseAssignees(int id)
        //{
        //    var assignees = await _context.CaseAssignees
        //        .Include(a => a.User)
        //        .Where(a => a.CaseId == id)
        //        .ToListAsync();

        //    if (!assignees.Any())
        //        return NotFound("No assignees found for this case.");

        //    return Ok(_mapper.Map<IEnumerable<CaseAssigneeDto>>(assignees));
        //}

        // ================================================================
        // GET: api/cases/{id}/evidence
        // Description: List all evidence for this case
        // ================================================================
        [HttpGet("GetCaseEvidences/{id:int}")]
        [Authorize(Roles = "Admin,Investigator,Officer")]
        public async Task<IActionResult> GetCaseEvidences(int id)
        {
            var evidences = await _context.Evidences
                .Include(e => e.AddedByUser)
                .Where(e => e.CaseId == id && !e.IsSoftDeleted)
                .ToListAsync();

            if (!evidences.Any())
                return NotFound("No evidence found for this case.");

            return Ok(_mapper.Map<IEnumerable<EvidenceReadDto>>(evidences));
        }

        // ================================================================
        // DELETE: api/cases/{id}
        // Description: Delete a case (Admin only)
        // ================================================================
        [HttpDelete("DeleteCase/{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCase(int id)
        {
            var existingCase = await _context.Cases.FindAsync(id);
            if (existingCase == null)
                return NotFound("Case not found.");

            _context.Cases.Remove(existingCase);
            await _context.SaveChangesAsync();

            return Ok($"Case '{existingCase.Name}' deleted successfully.");
        }
    }
}
