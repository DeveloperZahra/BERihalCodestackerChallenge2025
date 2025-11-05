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
    [Authorize] //  All authenticated users can access; restrictions inside methods
    public class EvidenceController : ControllerBase
    {
        private readonly EvidenceService _evidenceService;

        public EvidenceController(EvidenceService evidenceService)
        {
            _evidenceService = evidenceService;
        }

        // ================================================================
        // POST: api/evidence
        // Description: Add new evidence (Admin / Investigator / Officer)
        // ================================================================
        [HttpPost("AddTextEvidence")]
        [Authorize(Roles = "Admin,Investigator,Officer")]
        public async Task<IActionResult> AddTextEvidence([FromBody] EvidenceCreateDto dto)
        {
            if (dto == null || dto.CaseId <= 0)
                return BadRequest("Invalid input data.");

            int evidenceId = await _evidenceService.CreateTextAsync(dto.CaseId, dto.AddedByUserId, dto);
            return Ok($"Text evidence created successfully with ID: {evidenceId}");
        }

        // ================================================================
        // GET: api/evidence/{id}
        // Description: Get evidence by ID
        // ================================================================
        [HttpGet("GetEvidenceById/{id:int}")]
        public async Task<IActionResult> GetEvidenceById(int id)
        {
            var evidence = await _evidenceService.GetAsync(id);
            if (evidence == null)
                return NotFound("Evidence not found.");
            return Ok(evidence);
        }
        // ================================================================
        // GET: api/evidence/{id}/image
        // Description: Get image evidence info
        // ================================================================
        [HttpGet("GetImageEvidence/{id:int}")]
        public async Task<IActionResult> GetImageEvidence(int id)
        {
            var imageData = await _evidenceService.GetImageAsync(id);
            if (imageData == null)
                return BadRequest("Evidence not found or not an image.");

            return Ok(new
            {
                EvidenceId = id,
                MimeType = imageData?.mime,
                Message = "Image metadata retrieved successfully."
            });
        }


        // ================================================================
        // PUT: api/evidence/UpdateTextEvidence/{id}
        // Description: Update existing text evidence
        // ================================================================
        [HttpPut("UpdateTextEvidence/{id:int}")]
        [Authorize(Roles = "Admin,Investigator")]
        public async Task<IActionResult> UpdateTextEvidence(int id, [FromBody] EvidenceUpdateTextDto dto)
        {
            await _evidenceService.UpdateTextAsync(id, dto);
            return Ok($"Evidence {id} updated successfully.");
        }


        // ================================================================
        // PUT: api/evidence/UpdateImageEvidence/{id}
        // Description: Update existing image evidence
        // ================================================================
        [HttpPut("UpdateImageEvidence/{id:int}")]
        [Authorize(Roles = "Admin,Investigator")]
        public async Task<IActionResult> UpdateImageEvidence(int id, [FromBody] EvidenceUpdateImageDto dto)
        {
            await _evidenceService.UpdateImageAsync(id, dto);
            return Ok($"Image evidence {id} updated successfully.");
        }

        // ================================================================
        // DELETE (soft): api/evidence/{id}
        // Description: Soft delete evidence (mark as deleted)
        // ================================================================
        [HttpDelete("SoftDeleteEvidence/{id:int}")]
        [Authorize(Roles = "Admin,Investigator")]
        public async Task<IActionResult> SoftDeleteEvidence(int id, [FromQuery] int actedByUserId, [FromQuery] string? reason = null)
        {
            await _evidenceService.SoftDeleteAsync(id, actedByUserId, reason);
            return Ok($"Evidence {id} marked as soft deleted.");
        }

        // ================================================================
        // DELETE (hard): api/evidence/hard/{id}
        // Description:  (step 1): Start hard delete confirmation
        // ================================================================
        [HttpPost("StartHardDelete/{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> StartHardDelete(int id, [FromQuery] int actedByUserId)
        {
            var msg = await _evidenceService.StartHardDeleteAsync(id, actedByUserId);
            return Ok(msg);
        }
        // ================================================================
        // HARD DELETE (step 2): Confirm hard delete (yes/no)
        // ================================================================
        [HttpPost("ConfirmHardDelete/{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ConfirmHardDelete(int id, [FromQuery] int actedByUserId, [FromQuery] string answer)
        {
            var msg = await _evidenceService.ConfirmHardDeleteAsync(id, actedByUserId, answer);
            return Ok(msg);
        }
        // ================================================================
        // HARD DELETE (step 3): Finalize hard delete
        // ================================================================
        [HttpDelete("FinalizeHardDelete/{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> FinalizeHardDelete(int id, [FromQuery] int actedByUserId, [FromQuery] string command)
        {
            var success = await _evidenceService.FinalizeHardDeleteAsync(id, actedByUserId, command);
            if (!success)
                return BadRequest("Invalid or unconfirmed deletion command.");

            return Ok($"Evidence {id} permanently deleted.");
        }
    }
}
