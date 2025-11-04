using AutoMapper;
using BERihalCodestackerChallenge2025.Data;
using BERihalCodestackerChallenge2025.DTOs;
using BERihalCodestackerChallenge2025.Model;
using BERihalCodestackerChallenge2025.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BERihalCodestackerChallenge2025.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] //  Admin, Investigator, Officer (roles checked per-action)
    public class CasesController : ControllerBase
    {

        {
        }

        // ================================================================
        // POST: api/cases
        // Description: Create a new case and link to existing crime reports
        // ================================================================
        [HttpPost("CreateCase")]
        [Authorize(Roles = "Admin,Investigator")]
        public async Task<IActionResult> CreateCase([FromBody] CaseCreateDto dto)
        {

            var username = User.Identity?.Name;

            newCase.CreatedByUserId = creator.UserId;
            newCase.CaseNumber = $"CASE-{DateTime.UtcNow:yyyy}-{Guid.NewGuid().ToString().Substring(0, 6).ToUpper()}";


            {
                        });
                    }
                }
                await _context.SaveChangesAsync();
            }

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

        }


        // ================================================================
        // GET: api/cases
        // Description: Return list of all cases with 100-char trimmed description
        // ================================================================
        [HttpGet("GetAllCases")]
        [Authorize(Roles = "Admin,Investigator,Officer")]
        {
            var query = _context.Cases
                .Include(c => c.CreatedByUser)
                .AsQueryable();

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

        }
    }
}
