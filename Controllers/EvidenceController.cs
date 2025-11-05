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

        {
        }

        // ================================================================
        // POST: api/evidence
        // Description: Add new evidence (Admin / Investigator / Officer)
        // ================================================================
        [Authorize(Roles = "Admin,Investigator,Officer")]
        {

        }

        // ================================================================
        // GET: api/evidence/{id}
        // Description: Get evidence by ID
        // ================================================================
        [HttpGet("GetEvidenceById/{id:int}")]
        {
            if (evidence == null)
                return NotFound("Evidence not found.");
        }

        // ================================================================
        // GET: api/evidence/{id}/image
        // Description: Get image evidence info
        // ================================================================
        public async Task<IActionResult> GetImageEvidence(int id)
        {

            if (evidence.Type != EvidenceType.image)
                return BadRequest("This evidence is not an image type.");

            return Ok(new
            {
            });
        }


        // ================================================================
        // PUT: api/evidence/{id}
        // Description: Update existing evidence (content only)
        // ================================================================
        [Authorize(Roles = "Admin,Investigator")]
        {
            }
            if (evidence.Type == EvidenceType.text)
                evidence.TextContent = dto.TextContent;
            else if (evidence.Type == EvidenceType.image)
                evidence.FileUrl = dto.FileUrl;

            evidence.UpdatedAt = DateTime.UtcNow;

            {
        }

        // ================================================================
        // DELETE (soft): api/evidence/{id}
        // Description: Soft delete evidence (mark as deleted)
        // ================================================================
        [HttpDelete("SoftDeleteEvidence/{id:int}")]
        [Authorize(Roles = "Admin,Investigator")]
            {
        }

        // ================================================================
        // DELETE (hard): api/evidence/hard/{id}
        // Description: Permanently delete evidence (Admin only)
        // ================================================================
        [HttpDelete("HardDeleteEvidence/{id:int}")]
        [Authorize(Roles = "Admin")]
        {

            {

        }
    }
}
