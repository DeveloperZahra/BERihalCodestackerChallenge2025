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
        private readonly IEvidenceService _evidenceService;

        public EvidenceController(IEvidenceService evidenceService)
        {
            _evidenceService = evidenceService;
        }


        // ================================================================
        // POST: api/Evidence/AddTextEvidence
        // Description: Add new text-based evidence (Admin / Investigator / Officer)
        // ================================================================
        [HttpPost("AddTextEvidence")]
        [Authorize(Roles = "Admin, Investigator, Officer")]
        public async Task<IActionResult> AddTextEvidence([FromBody] EvidenceCreateDto dto, CancellationToken ct)
        {
            try
            {
                // ================================================================
                // Step 1: Validate Input
                // ================================================================
                if (dto == null)
                    return BadRequest(new { Message = "Request body cannot be null." });

                if (dto.CaseId <= 0)
                    return BadRequest(new { Message = "Invalid Case ID. It must be greater than zero." });

                if (dto.AddedByUserId <= 0)
                    return BadRequest(new { Message = "Invalid User ID. It must be greater than zero." });

                if (string.IsNullOrWhiteSpace(dto.TextContent))
                    return BadRequest(new { Message = "Evidence content cannot be empty." });

                // ================================================================
                // Step 2: Call the Service Layer
                // ================================================================
                int evidenceId = await _evidenceService.CreateTextAsync(dto.CaseId, dto.AddedByUserId, dto, ct);

                // ================================================================
                // Step 3: Return Success Response
                // ================================================================
                return Ok(new
                {
                    Message = "Text evidence created successfully.",
                    EvidenceId = evidenceId,
                    dto.CaseId,
                    dto.AddedByUserId,
                    dto.TextContent,
                    CreatedAt = DateTime.UtcNow
                });
            }

            // ------------------- ERROR HANDLING -------------------

            catch (ArgumentException ex)
            {
                // Invalid input or enum value
                return BadRequest(new
                {
                    Message = "Invalid argument provided.",
                    Details = ex.Message
                });
            }
            catch (KeyNotFoundException ex)
            {
                // Case or user not found
                return NotFound(new
                {
                    Message = "Referenced entity not found (case or user).",
                    Details = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                // Logical or duplicate entry issues
                return Conflict(new
                {
                    Message = "A conflict occurred while creating evidence.",
                    Details = ex.Message
                });
            }
            catch (DbUpdateException ex)
            {
                // Database constraint or foreign key problem
                return StatusCode(500, new
                {
                    Message = "Database error occurred while adding evidence.",
                    Details = ex.InnerException?.Message ?? ex.Message
                });
            }
            catch (Exception ex)
            {
                // Any unexpected exception
                return StatusCode(500, new
                {
                    Message = "An unexpected error occurred while creating text evidence.",
                    Details = ex.Message
                });
            }
        }


        // ================================================================
        // GET: api/Evidence/GetEvidenceById/{id}
        // Description: Retrieve a specific evidence record by its ID
        // ================================================================
        [HttpGet("GetEvidenceById")]
        [Authorize(Roles = "Admin, Investigator, Officer")]
        public async Task<IActionResult> GetEvidenceById(int id, CancellationToken ct)
        {
            try
            {
                // ================================================================
                // Step 1: Validate input
                // ================================================================
                if (id <= 0)
                    return BadRequest(new { Message = "Invalid Evidence ID. It must be greater than zero." });

                // ================================================================
                // Step 2: Retrieve evidence using the service
                // ================================================================
                var evidence = await _evidenceService.GetAsync(id, ct);

                // ================================================================
                // Step 3: Handle not found case
                // ================================================================
                if (evidence == null)
                    return NotFound(new { Message = $"Evidence with ID {id} was not found." });

                // ================================================================
                // Step 4: Return success response
                // ================================================================
                return Ok(new
                {
                    Message = "Evidence retrieved successfully.",
                    Evidence = evidence
                });
            }

            // ------------------- ERROR HANDLING -------------------

            catch (ArgumentException ex)
            {
                // Invalid argument (for example, wrong ID format)
                return BadRequest(new
                {
                    Message = "Invalid input value provided.",
                    Details = ex.Message
                });
            }
            catch (KeyNotFoundException ex)
            {
                // Evidence not found in service layer
                return NotFound(new
                {
                    Message = "Evidence record not found.",
                    Details = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                // EF or logic-level issue
                return StatusCode(500, new
                {
                    Message = "Invalid operation occurred while retrieving evidence.",
                    Details = ex.Message
                });
            }
            catch (DbUpdateException ex)
            {
                // Database connection or constraint issue
                return StatusCode(500, new
                {
                    Message = "Database error occurred while retrieving evidence.",
                    Details = ex.InnerException?.Message ?? ex.Message
                });
            }
            catch (Exception ex)
            {
                // Any unexpected error
                return StatusCode(500, new
                {
                    Message = "An unexpected error occurred while retrieving evidence.",
                    Details = ex.Message
                });
            }
        }
       
        // ================================================================
        // PUT: api/evidence/UpdateTextEvidence/{id}
        // Description: Update existing text evidence
        // Roles: Admin, Investigator
        // ================================================================
        [HttpPut("UpdateTextEvidence")]
        [Authorize(Roles = "Admin, Investigator")]
        public async Task<IActionResult> UpdateTextEvidence(int id, [FromBody] EvidenceUpdateTextDto dto, CancellationToken ct)
        {
            try
            {
                // Step 1: Validate input
                if (id <= 0)
                    return BadRequest(new { Message = "Invalid Evidence ID. It must be greater than zero." });

                if (dto == null)
                    return BadRequest(new { Message = "Invalid request body. DTO cannot be null." });

                // Step 2: Attempt to update evidence
                await _evidenceService.UpdateTextAsync(id, dto, ct);

                // Step 3: Return success response
                return Ok(new
                {
                    Message = $"Evidence with ID {id} updated successfully.",
                    UpdatedAt = DateTime.UtcNow
                });
            }
            catch (KeyNotFoundException ex)
            {
                // Evidence not found
                return NotFound(new
                {
                    Message = "Evidence not found.",
                    Details = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                // Validation issues (invalid input)
                return BadRequest(new
                {
                    Message = "Invalid argument provided.",
                    Details = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                // Trying to update a non-text evidence
                return StatusCode(409, new
                {
                    Message = "Invalid operation. Cannot update non-text evidence.",
                    Details = ex.Message
                });
            }
            catch (DbUpdateException ex)
            {
                // Database issues
                return StatusCode(500, new
                {
                    Message = "Database error occurred while updating evidence.",
                    Details = ex.InnerException?.Message ?? ex.Message
                });
            }
            catch (Exception ex)
            {
                // Unexpected error
                return StatusCode(500, new
                {
                    Message = "An unexpected error occurred while updating text evidence.",
                    Details = ex.Message
                });
            }
        }

        // ================================================================
        // DELETE (soft): api/evidence/SoftDeleteEvidence/{id}
        // Description: Soft delete evidence (mark as deleted)
        // Roles: Admin, Investigator
        // ================================================================
        [HttpDelete("SoftDeleteEvidence")]
        [Authorize(Roles = "Admin, Investigator")]
        public async Task<IActionResult> SoftDeleteEvidence(int id, [FromQuery] int actedByUserId, [FromQuery] string? reason = null, CancellationToken ct = default)
        {
            try
            {
                // Step 1: Validate input parameters
                if (id <= 0)
                    return BadRequest(new { Message = "Invalid Evidence ID. It must be greater than zero." });

                if (actedByUserId <= 0)
                    return BadRequest(new { Message = "Invalid User ID. It must be greater than zero." });

                // Step 2: Attempt soft delete
                await _evidenceService.SoftDeleteAsync(id, actedByUserId, reason, ct);

                // Step 3: Return success response
                return Ok(new
                {
                    Message = $"Evidence with ID {id} marked as soft deleted successfully.",
                    DeletedBy = actedByUserId,
                    Reason = reason ?? "No reason provided.",
                    DeletedAt = DateTime.UtcNow
                });
            }
            catch (KeyNotFoundException ex)
            {
                // Evidence not found
                return NotFound(new
                {
                    Message = "Evidence not found.",
                    Details = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                // Validation or argument issue
                return BadRequest(new
                {
                    Message = "Invalid input argument.",
                    Details = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                // Operation not allowed (e.g., evidence already deleted)
                return StatusCode(409, new
                {
                    Message = "Invalid operation. Evidence may already be soft deleted.",
                    Details = ex.Message
                });
            }
            catch (DbUpdateException ex)
            {
                // Database error
                return StatusCode(500, new
                {
                    Message = "Database error occurred while performing soft delete.",
                    Details = ex.InnerException?.Message ?? ex.Message
                });
            }
            catch (Exception ex)
            {
                // Generic error
                return StatusCode(500, new
                {
                    Message = "An unexpected error occurred while performing soft delete.",
                    Details = ex.Message
                });
            }
        }


        // ================================================================
        // DELETE (hard): api/evidence/hard/{id}
        // Description: (Step 1) Start hard delete confirmation
        // ================================================================
        [HttpPost("StartHardDelete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> StartHardDelete(int id, [FromQuery] int actedByUserId, CancellationToken ct = default)
        {
            try
            {
                // Step 1: Validate input
                if (id <= 0)
                    return BadRequest(new { Message = "Invalid Evidence ID. It must be greater than zero." });

                if (actedByUserId <= 0)
                    return BadRequest(new { Message = "Invalid User ID. It must be greater than zero." });

                // Step 2: Call service
                var msg = await _evidenceService.StartHardDeleteAsync(id, actedByUserId);

                // Step 3: Return success response
                return Ok(new
                {
                    Message = msg,
                    StartedAt = DateTime.UtcNow
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = "Evidence not found.", Details = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = "Invalid argument provided.", Details = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An unexpected error occurred while starting hard delete.", Details = ex.Message });
            }
        }



        // ================================================================
        // HARD DELETE (step 2): Confirm hard delete (yes/no)
        // ================================================================
        [HttpPost("ConfirmHardDelete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ConfirmHardDelete(int id, [FromQuery] int actedByUserId, [FromQuery] string answer, CancellationToken ct = default)
        {
            try
            {
                // Step 1: Validate input
                if (id <= 0)
                    return BadRequest(new { Message = "Invalid Evidence ID. It must be greater than zero." });

                if (actedByUserId <= 0)
                    return BadRequest(new { Message = "Invalid User ID. It must be greater than zero." });

                if (string.IsNullOrWhiteSpace(answer))
                    return BadRequest(new { Message = "Answer (yes/no) is required." });

                // Step 2: Call service
                var msg = await _evidenceService.ConfirmHardDeleteAsync(id, actedByUserId, answer);

                // Step 3: Return confirmation message
                return Ok(new
                {
                    Message = msg,
                    ConfirmedAt = DateTime.UtcNow
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = "Evidence not found or no delete session started.", Details = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = "Invalid argument provided.", Details = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(409, new { Message = "Invalid operation during confirmation.", Details = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An unexpected error occurred during hard delete confirmation.", Details = ex.Message });
            }
        }



        // ================================================================
        // HARD DELETE (step 3): Finalize hard delete
        // ================================================================
        [HttpDelete("FinalizeHardDelete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> FinalizeHardDelete(int id, [FromQuery] int actedByUserId, [FromQuery] string command, CancellationToken ct = default)
        {
            try
            {
                // Step 1: Validate inputs
                if (id <= 0)
                    return BadRequest(new { Message = "Invalid Evidence ID. It must be greater than zero." });

                if (actedByUserId <= 0)
                    return BadRequest(new { Message = "Invalid User ID. It must be greater than zero." });

                if (string.IsNullOrWhiteSpace(command))
                    return BadRequest(new { Message = "Command is required (expected format: DELETE {id})." });

                // Step 2: Call service
                var success = await _evidenceService.FinalizeHardDeleteAsync(id, actedByUserId, command, ct);

                // Step 3: Handle invalid or unconfirmed deletion
                if (!success)
                    return BadRequest(new { Message = "Invalid or unconfirmed deletion command." });

                // Step 4: Success response
                return Ok(new
                {
                    Message = $"Evidence with ID {id} permanently deleted.",
                    DeletedBy = actedByUserId,
                    DeletedAt = DateTime.UtcNow
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = "Evidence not found.", Details = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = "Invalid argument provided.", Details = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(409, new { Message = "Invalid operation. Deletion could not be completed.", Details = ex.Message });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { Message = "Database error occurred while finalizing hard delete.", Details = ex.InnerException?.Message ?? ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An unexpected error occurred while finalizing hard delete.", Details = ex.Message });
            }
        }

    }
}
