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
    [Authorize(Roles = "Admin,Investigator")]
    public class CaseAssigneesController : ControllerBase
    {
        private readonly ICaseAssigneeService _service;
        public CaseAssigneesController(ICaseAssigneeService service)
        {
            _service = service;
        }


        // ======================================================
        // POST: api/CaseAssignees/AssignUser
        // Description: Assign a user to a specific case
        // ======================================================
        [HttpPost("AssignUser")]
        [Authorize(Roles = "Admin, Investigator")]
        public async Task<IActionResult> AssignUserToCase([FromBody] CaseAssigneeCreateDto dto, CancellationToken ct)
        {
            try
            {
                // ================================================================
                // Step 1: Validate Input
                // ================================================================
                if (dto == null)
                    return BadRequest(new { Message = "Request body cannot be null." });

                if (dto.CaseId <= 0 || dto.UserId <= 0)
                    return BadRequest(new { Message = "Invalid CaseId or UserId. Both must be positive integers." });

                // ================================================================
                // Step 2: Assign user using Service (it handles all logic inside)
                // ================================================================
                var result = await _service.AssignAsync(dto, ct);

                // ================================================================
                // Step 3: Return success response
                // ================================================================
                return Ok(new
                {
                    Message = "User assigned successfully to the case.",
                    Result = result
                });
            }
            // ------------------- Error Handling -------------------
            catch (ArgumentException ex)
            {
                // Input validation or invalid enum values
                return BadRequest(new
                {
                    Message = "Invalid input detected.",
                    Details = ex.Message
                });
            }
            catch (KeyNotFoundException ex)
            {
                // Case or User not found
                return NotFound(new
                {
                    Message = "Referenced entity not found.",
                    Details = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                // Duplicate assignment or logical error
                return Conflict(new
                {
                    Message = "Conflict occurred during assignment.",
                    Details = ex.Message
                });
            }
            catch (DbUpdateException ex)
            {
                // Database-level error
                return StatusCode(500, new
                {
                    Message = "Database error occurred while assigning user to case.",
                    Details = ex.InnerException?.Message ?? ex.Message
                });
            }
            catch (Exception ex)
            {
                // Fallback for any unexpected issue
                return StatusCode(500, new
                {
                    Message = "An unexpected error occurred while assigning user to the case.",
                    Details = ex.Message
                });
            }
        }
        // ============================================================
        //  Get all assignees for a specific case
        // ============================================================
        [HttpGet("GetAllAssignees")]
        [Authorize(Roles = "Admin, Investigator")]
        public async Task<IActionResult> GetAssigneesByCase(int caseId, CancellationToken ct)
        {
            try
            {
                // ================================================================
                // Step 1: Validate input
                // ================================================================
                if (caseId <= 0)
                    return BadRequest(new { Message = "Invalid Case ID. It must be greater than zero." });

                // ================================================================
                // Step 2: Retrieve data via Service
                // ================================================================
                var assignees = await _service.GetByCaseAsync(caseId, ct);

                // ================================================================
                // Step 3: Handle empty or null result
                // ================================================================
                if (assignees == null || !assignees.Any())
                    return NotFound(new { Message = $"No assignees found for case ID {caseId}." });

                // ================================================================
                // Step 4: Return success response
                // ================================================================
                return Ok(new
                {
                    Message = "Assignees retrieved successfully.",
                    CaseId = caseId,
                    Count = assignees.Count,
                    Assignees = assignees
                });
            }

            // ------------------- Error Handling -------------------
            catch (ArgumentException ex)
            {
                // Invalid CaseId or bad input
                return BadRequest(new
                {
                    Message = "Invalid input detected.",
                    Details = ex.Message
                });
            }
            catch (KeyNotFoundException ex)
            {
                // Case not found in database
                return NotFound(new
                {
                    Message = "Case not found.",
                    Details = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                // EF or logical inconsistency
                return StatusCode(500, new
                {
                    Message = "An invalid operation occurred while retrieving assignees.",
                    Details = ex.Message
                });
            }
            catch (DbUpdateException ex)
            {
                // Database connection or constraint issue
                return StatusCode(500, new
                {
                    Message = "Database error occurred while accessing assignee data.",
                    Details = ex.InnerException?.Message ?? ex.Message
                });
            }
            catch (Exception ex)
            {
                // Unexpected error
                return StatusCode(500, new
                {
                    Message = "An unexpected error occurred while retrieving assignees.",
                    Details = ex.Message
                });
            }
        }

        // ================================================================
        // Description: Update progress status (Investigator / Admin only)
        // ================================================================
        [Authorize(Roles = "Investigator, Admin")]
        [HttpPut("UpdateCaseProgress")]
        public async Task<IActionResult> UpdateProgressStatus(int assigneeId, [FromBody] CaseProgressUpdateDto dto, CancellationToken ct)
        {
            try
            {
                // ================================================================
                // Step 1: Validate input
                // ================================================================
                if (dto == null)
                    return BadRequest(new { Message = "Request body cannot be null." });

                if (string.IsNullOrWhiteSpace(dto.ProgressStatus))
                    return BadRequest(new { Message = "ProgressStatus value is required." });

                if (assigneeId <= 0)
                    return BadRequest(new { Message = "Invalid Assignee ID. It must be greater than zero." });

                // ================================================================
                // Step 2: Call the service to update progress
                // ================================================================
                var updated = await _service.UpdateProgressAsync(assigneeId, dto, ct);

                // ================================================================
                // Step 3: Handle not found or null result
                // ================================================================
                if (updated == null)
                    return NotFound(new { Message = $"Assignee with ID {assigneeId} not found." });

                // ================================================================
                // Step 4: Return success response
                // ================================================================
                return Ok(new
                {
                    Message = "Progress status updated successfully.",
                    UpdatedAssignee = updated
                });
            }

            // -------------------- ERROR HANDLING SECTION --------------------

            catch (ArgumentException ex)
            {
                // Invalid enum or argument issues
                return BadRequest(new
                {
                    Message = "Invalid ProgressStatus value or input.",
                    Details = ex.Message,
                    AllowedValues = Enum.GetNames(typeof(CaseStatus))
                });
            }
            catch (KeyNotFoundException ex)
            {
                // Case or Assignee not found
                return NotFound(new
                {
                    Message = "Assignee or related case not found.",
                    Details = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                // Logical or EF context issue
                return StatusCode(500, new
                {
                    Message = "Invalid operation during progress update.",
                    Details = ex.Message
                });
            }
            catch (DbUpdateException ex)
            {
                // Database constraint or FK issue
                return StatusCode(500, new
                {
                    Message = "Database error occurred while updating case progress.",
                    Details = ex.InnerException?.Message ?? ex.Message
                });
            }
            catch (Exception ex)
            {
                // Any unexpected error
                return StatusCode(500, new
                {
                    Message = "An unexpected error occurred while updating progress status.",
                    Details = ex.Message
                });
            }
        }

        // ================================================================
        // DELETE: api/CaseAssignees/DeleteAssignee/{id}
        // Description: Remove an assignee from a case (Admin / Investigator)
        // ================================================================
        [Authorize(Roles = "Admin, Investigator")]
        [HttpDelete("DeleteAssignee")]
        public async Task<IActionResult> RemoveAssignee(int id, CancellationToken ct)
        {
            try
            {
                // ================================================================
                // Step 1: Validate input
                // ================================================================
                if (id <= 0)
                    return BadRequest(new { Message = "Invalid Assignee ID. It must be greater than zero." });

                // ================================================================
                // Step 2: Call service to remove the assignee
                // ================================================================
                var removed = await _service.RemoveAsync(id, ct);

                // ================================================================
                // Step 3: Handle not found result
                // ================================================================
                if (!removed)
                    return NotFound(new { Message = $"Assignee with ID {id} not found." });

                // ================================================================
                // Step 4: Return success response
                // ================================================================
                return Ok(new
                {
                    Message = "Assignee removed successfully.",
                    AssigneeId = id,
                    RemovedAt = DateTime.UtcNow
                });
            }

            // ------------------- ERROR HANDLING -------------------

            catch (ArgumentException ex)
            {
                // Input validation error
                return BadRequest(new
                {
                    Message = "Invalid input value provided.",
                    Details = ex.Message
                });
            }
            catch (KeyNotFoundException ex)
            {
                // Assignee or related entity not found
                return NotFound(new
                {
                    Message = "Assignee not found.",
                    Details = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                // EF or logic issues
                return StatusCode(500, new
                {
                    Message = "Invalid operation during assignee removal.",
                    Details = ex.Message
                });
            }
            catch (DbUpdateException ex)
            {
                // Database-level errors (e.g., constraint issues)
                return StatusCode(500, new
                {
                    Message = "Database error occurred while removing assignee.",
                    Details = ex.InnerException?.Message ?? ex.Message
                });
            }
            catch (Exception ex)
            {
                // Unexpected error
                return StatusCode(500, new
                {
                    Message = "An unexpected error occurred while removing the assignee.",
                    Details = ex.Message
                });
            }
        }

        // ================================================================
        // Helper: Validate clearance level rule
        // ================================================================
        private bool HasValidClearance(string userClearance, string caseClearance)
            {
                var levels = new[] { "low", "medium", "high", "critical" };
                int userIndex = Array.IndexOf(levels, userClearance.ToLower());
                int caseIndex = Array.IndexOf(levels, caseClearance.ToLower());
                return userIndex >= caseIndex; //  User must have >= clearance
            }
        }
    }

