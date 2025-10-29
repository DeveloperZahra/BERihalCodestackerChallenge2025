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
    [Authorize] //  All authenticated users can access; restrictions inside methods
    public class EvidenceController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public EvidenceController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // ================================================================
        // POST: api/evidence
        // Description: Add new evidence (Admin / Investigator / Officer)
        // ================================================================
        [HttpPost("AddEvidence")]
        [Authorize(Roles = "Admin,Investigator,Officer")]
        public async Task<IActionResult> AddEvidence([FromBody] EvidenceCreateDto dto)
        {
            //  Validate case existence
            var caseEntity = await _context.Cases.FindAsync(dto.CaseId);
            if (caseEntity == null)
                return NotFound("Case not found.");

            //  Validate evidence type
            if (dto.Type != "text" && dto.Type != "image")
                return BadRequest("Invalid evidence type. Must be 'text' or 'image'.");

            // Validate image fields if image
            if (dto.Type == "image" && string.IsNullOrWhiteSpace(dto.FileUrl))
                return BadRequest("Image evidence must include FileUrl.");

            //  Create evidence object
            var evidence = _mapper.Map<Evidence>(dto);
            evidence.CreatedAt = DateTime.UtcNow;
            evidence.IsSoftDeleted = false;

            _context.Evidences.Add(evidence);

            //  Create audit log
            var log = new EvidenceAuditLog
            {
                Action = "add",
                ActedAt = DateTime.UtcNow,
                Details = $"Evidence added to case {dto.CaseId}",
                ActedByUserId = evidence.AddedByUserId
            };
            _context.EvidenceAuditLogs.Add(log);

            await _context.SaveChangesAsync();
            var result = _mapper.Map<EvidenceReadDto>(evidence);

            return CreatedAtAction(nameof(GetEvidenceById), new { id = evidence.Id }, result);
        }

        // ================================================================
        // GET: api/evidence/{id}
        // Description: Get evidence by ID
        // ================================================================
        [HttpGet("GetEvidenceById/{id:int}")]
        public async Task<ActionResult<EvidenceReadDto>> GetEvidenceById(int id)
        {
            var evidence = await _context.Evidences
                .Include(e => e.AddedByUser)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);

            if (evidence == null)
                return NotFound("Evidence not found.");

            var dto = _mapper.Map<EvidenceReadDto>(evidence);
            return Ok(dto);
        }

        // ================================================================
        // GET: api/evidence/{id}/image
        // Description: Get image evidence info
        // ================================================================
        [HttpGet("GetImageEvidence{id:int}")]
        public async Task<IActionResult> GetImageEvidence(int id)
        {
            var evidence = await _context.Evidences.FindAsync(id);
            if (evidence == null)
                return NotFound("Evidence not found.");

            if (evidence.Type != EvidenceType.image)
                return BadRequest("This evidence is not an image type.");

            return Ok(new
            {
                evidence.Id,
                evidence.FileUrl,
                evidence.MimeType,
                evidence.SizeBytes
            });
        }

        // ================================================================
        // PUT: api/evidence/{id}
        // Description: Update existing evidence (content only)
        // ================================================================
        [HttpPut("UpdateEvidence/{id:int}")]
        [Authorize(Roles = "Admin,Investigator")]
        public async Task<IActionResult> UpdateEvidence(int id, [FromBody] EvidenceCreateDto dto)
        {
            var evidence = await _context.Evidences.FindAsync(id);
            if (evidence == null)
                return NotFound("Evidence not found.");

            // Cannot change evidence type
            if (dto.Type != null &&
            Enum.TryParse<EvidenceType>(dto.Type, true, out var parsedType) &&
            parsedType != evidence.Type)
            {
                return BadRequest("Cannot change evidence type.");
            }
            if (evidence.Type == EvidenceType.text)
                evidence.TextContent = dto.TextContent;
            else if (evidence.Type == EvidenceType.image)
                evidence.FileUrl = dto.FileUrl;

            evidence.UpdatedAt = DateTime.UtcNow;

            //  Audit log
            var log = new EvidenceAuditLog
            {
                Action = "update",
                ActedAt = DateTime.UtcNow,
                Details = $"Evidence {id} updated.",
                ActedByUserId = evidence.AddedByUserId
            };
            _context.EvidenceAuditLogs.Add(log);

            await _context.SaveChangesAsync();
            return Ok("Evidence updated successfully.");
        }

        // ================================================================
        // DELETE (soft): api/evidence/{id}
        // Description: Soft delete evidence (mark as deleted)
        // ================================================================
        [HttpDelete("SoftDeleteEvidence/{id:int}")]
        [Authorize(Roles = "Admin,Investigator")]
        public async Task<IActionResult> SoftDeleteEvidence(int id)
        {
            var evidence = await _context.Evidences.FindAsync(id);
            if (evidence == null)
                return NotFound("Evidence not found.");

            evidence.IsSoftDeleted = true;
            evidence.UpdatedAt = DateTime.UtcNow;

            //  Audit log
            var log = new EvidenceAuditLog
            {
                Action = "soft_delete",
                ActedAt = DateTime.UtcNow,
                Details = $"Evidence {id} marked as soft deleted.",
                ActedByUserId = evidence.AddedByUserId
            };
            _context.EvidenceAuditLogs.Add(log);

            await _context.SaveChangesAsync();
            return Ok($"Evidence {id} soft deleted successfully.");
        }

        // ================================================================
        // DELETE (hard): api/evidence/hard/{id}
        // Description: Permanently delete evidence (Admin only)
        // ================================================================
        [HttpDelete("HardDeleteEvidence/{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> HardDeleteEvidence(int id, [FromQuery] string confirm = "")
        {
            var evidence = await _context.Evidences.FindAsync(id);
            if (evidence == null)
                return NotFound("Evidence not found.");

            if (confirm.ToLower() != "yes")
                return BadRequest($"Are you sure you want to permanently delete Evidence ID: {id}? Add '?confirm=yes' to proceed.");

            //  Audit log
            var log = new EvidenceAuditLog
            {
                Action = "hard_delete",
                ActedAt = DateTime.UtcNow,
                Details = $"Evidence {id} permanently deleted.",
                ActedByUserId = evidence.AddedByUserId
            };
            _context.EvidenceAuditLogs.Add(log);

            _context.Evidences.Remove(evidence);
            await _context.SaveChangesAsync();

            return Ok($"Evidence ID {id} permanently deleted.");
        }
    }
}
