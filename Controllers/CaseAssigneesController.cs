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
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Investigator")]
    public class CaseAssigneesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public CaseAssigneesController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // ======================================================
        // POST: api/CaseAssignees/AssignUserToCase
        // Description: Assign a user to a specific case
        // ======================================================
        [HttpPost("AssignUser")]
        [Authorize(Roles = "Admin, Investigator")] //Only the investigator or supervisor can be allowed.
        public IActionResult AssignUserToCase([FromBody] CaseAssigneeCreateDto dto)
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
                //  Step 2: Check Case and User existence
                // ================================================================
                var caseEntity = _context.Cases.FirstOrDefault(c => c.CaseId == dto.CaseId);
                var userEntity = _context.Users.FirstOrDefault(u => u.UserId == dto.UserId);

                if (caseEntity == null)
                    return NotFound(new { Message = $"Case with ID {dto.CaseId} not found." });

                if (userEntity == null)
                    return NotFound(new { Message = $"User with ID {dto.UserId} not found." });

                // ================================================================
                //  Step 3: Prevent duplicate assignments
                // ================================================================
                bool alreadyAssigned = _context.CaseAssignees.Any(a => a.CaseId == dto.CaseId && a.UserId == dto.UserId);
                if (alreadyAssigned)
                    return Conflict(new
                    {
                        Message = "User is already assigned to this case.",
                        Details = $"UserId {dto.UserId} is already linked to CaseId {dto.CaseId}."
                    });

                // ================================================================
                //  Step 4: Validate Clearance Level Enum (optional but recommended)
                // ================================================================
                if (!Enum.TryParse<Clearance>(dto.ClearanceLevel, true, out var level))
                    return BadRequest(new
                    {
                        Message = "Invalid ClearanceLevel value.",
                        AllowedValues = Enum.GetNames(typeof(Clearance))
                    });

                // ================================================================
                //  Step 5: Create and save the assignment
                // ================================================================
                var assignment = new CaseAssignee
                {
                    CaseId = dto.CaseId,
                    UserId = dto.UserId,
                    AssignedAt = DateTime.UtcNow,
                    ClearanceLevel = dto.ClearanceLevel
                };

                _context.CaseAssignees.Add(assignment);
                _context.SaveChanges();

                // ================================================================
                //  Step 6: Return success response
                // ================================================================
                return Ok(new
                {
                    Message = "User assigned successfully to the case.",
                    CaseAssigneeId = assignment.CaseAssigneeId,
                    assignment.CaseId,
                    assignment.UserId,
                    assignment.ClearanceLevel,
                    AssignedAt = assignment.AssignedAt
                });
            }
            catch (DbUpdateException ex)
            {
                // Database error (FK or PK constraints)
                return StatusCode(500, new
                {
                    Message = "Database error occurred while assigning user to case.",
                    Details = ex.InnerException?.Message ?? ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                //Logical error (such as a problem in EF or Context)
                return StatusCode(500, new
                {
                    Message = "Operation failed due to invalid state.",
                    Details = ex.Message
                });
            }
            catch (Exception ex)
            {
                // Any unexpected error
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
        public IActionResult GetAssigneesByCase(int caseId)
        {
            try
            {
                // ================================================================
                //  Step 1: Validate the input parameter
                // ================================================================
                if (caseId <= 0)
                    return BadRequest(new { Message = "Invalid Case ID. It must be greater than zero." });

                // ================================================================
                // Step 2: Check if the case exists
                // ================================================================
                var caseExists = _context.Cases.Any(c => c.CaseId == caseId);
                if (!caseExists)
                    return NotFound(new { Message = $"Case with ID {caseId} not found." });

                // ================================================================
                //  Step 3: Retrieve all assignees for this case
                // ================================================================
                var assignees = _context.CaseAssignees
                    .Include(a => a.User) // ✅ Optional: load related user details
                    .Where(a => a.CaseId == caseId)
                    .Select(a => new
                    {
                        a.CaseAssigneeId,
                        a.CaseId,
                        a.UserId,
                        UserFullName = a.User.FullName, //  Associated username
                        UserRole = a.User.Role,
                        a.AssignedAt,
                        a.ClearanceLevel
                    })
                    .ToList();

                // ================================================================
                //  Step 4: Handle empty result
                // ================================================================
                if (assignees == null || !assignees.Any())
                    return NotFound(new { Message = $"No assignees found for case ID {caseId}." });

                // ================================================================
                //  Step 5: Return success response
                // ================================================================
                return Ok(new
                {
                    Message = "Assignees retrieved successfully.",
                    CaseId = caseId,
                    Count = assignees.Count,
                    Assignees = assignees
                });
            }
            catch (InvalidOperationException ex)
            {
                //  Problem with EF or query logic
                return StatusCode(500, new
                {
                    Message = "An invalid operation occurred while retrieving assignees.",
                    Details = ex.Message
                });
            }
            catch (DbUpdateException ex)
            {
                //  Problems connecting to the database
                return StatusCode(500, new
                {
                    Message = "Database error occurred while accessing assignee data.",
                    Details = ex.InnerException?.Message ?? ex.Message
                });
            }
            catch (Exception ex)
            {
                //  Any unexpected error
                return StatusCode(500, new
                {
                    Message = "An unexpected error occurred while retrieving assignees.",
                    Details = ex.Message
                });
            }
        }


        // ================================================================
        // Description: Update progress status (Officer only)
        // ================================================================
        [Authorize(Roles = "Investigator, Admin")]
        [HttpPut("UpdateCaseProgress")]
        public async Task<IActionResult> UpdateProgressStatus(int assigneeId, [FromBody] CaseProgressUpdateDto dto)
        {
            try
            {
                // ================================================================
                //  Step 1: Validate input
                // ================================================================
                if (dto == null)
                    return BadRequest(new { Message = "Request body cannot be null." });

                if (string.IsNullOrWhiteSpace(dto.ProgressStatus))
                    return BadRequest(new { Message = "ProgressStatus value is required." });

                if (assigneeId <= 0)
                    return BadRequest(new { Message = "Invalid assignee ID. It must be greater than zero." });

                // ================================================================
                // Step 2: Retrieve assignee with related case and user
                // ================================================================
                var assignee = await _context.CaseAssignees
                    .Include(a => a.Case)
                    .Include(a => a.User)
                    .FirstOrDefaultAsync(a => a.CaseAssigneeId == assigneeId);

                if (assignee == null)
                    return NotFound(new { Message = $"Assignee with ID {assigneeId} not found." });

                // ================================================================
                //  Step 3: Validate the provided status value (Enum check)
                // ================================================================
                if (!Enum.TryParse<CaseStatus>(dto.ProgressStatus, true, out var newStatus))
                {
                    return BadRequest(new
                    {
                        Message = "Invalid ProgressStatus value.",
                        AllowedValues = Enum.GetNames(typeof(CaseStatus))
                    });
                }

                // ================================================================
                //  Step 4: Update the progress status
                // ================================================================
                assignee.ProgressStatus = newStatus;
                assignee.UpdatedAt = DateTime.UtcNow; // Optional: track time

                await _context.SaveChangesAsync();

                // ================================================================
                //  Step 5: Return success response
                // ================================================================
                return Ok(new
                {
                    Message = "Progress status updated successfully.",
                    AssigneeId = assignee.CaseAssigneeId,
                    CaseName = assignee.Case?.Name,
                    User = assignee.User?.FullName ?? "Unknown",
                    NewStatus = newStatus.ToString(),
                    UpdatedAt = assignee.UpdatedAt
                });
            }
            catch (DbUpdateException ex)
            {
                //  Database constraint or FK issue
                return StatusCode(500, new
                {
                    Message = "Database error occurred while updating case progress.",
                    Details = ex.InnerException?.Message ?? ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                //  Logical or EF context issue
                return StatusCode(500, new
                {
                    Message = "Invalid operation during progress update.",
                    Details = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                //  Enum.Parse or invalid argument issue
                return BadRequest(new
                {
                    Message = "Invalid input value provided.",
                    Details = ex.Message
                });
            }
            catch (Exception ex)
            {
                //  Catch all unexpected errors
                return StatusCode(500, new
                {
                    Message = "An unexpected error occurred while updating progress status.",
                    Details = ex.Message
                });
            }
        }


        // ================================================================
        // DELETE: api/caseassignees/{id}
        // Description: Remove an assignee from a case (Admin/Investigator)
        // ================================================================
        [Authorize(Roles = "Admin, Investigator")] 
        [HttpDelete("DeleteAssignee")]
        public async Task<IActionResult> RemoveAssignee(int id)
        {
            try
            {
                // ================================================================
                //  Step 1: Validate input
                // ================================================================
                if (id <= 0)
                    return BadRequest(new { Message = "Invalid Assignee ID. It must be greater than zero." });

                // ================================================================
                //  Step 2: Retrieve the assignee from DB
                // ================================================================
                var assignee = await _context.CaseAssignees
                    .Include(a => a.Case)
                    .Include(a => a.User)
                    .FirstOrDefaultAsync(a => a.CaseAssigneeId == id);

                if (assignee == null)
                    return NotFound(new { Message = $"Assignee with ID {id} not found." });

                // ================================================================
                //  Step 3: Remove the assignee
                // ================================================================
                _context.CaseAssignees.Remove(assignee);
                await _context.SaveChangesAsync();

                // ================================================================
                //  Step 4: Return success response
                // ================================================================
                return Ok(new
                {
                    Message = "Assignee removed successfully.",
                    AssigneeId = id,
                    Case = assignee.Case?.Name ?? "Unknown case",
                    User = assignee.User?.FullName ?? "Unknown user",
                    RemovedAt = DateTime.UtcNow
                });
            }
            catch (DbUpdateException ex)
            {
                //  Database update issue (e.g., foreign key constraint)
                return StatusCode(500, new
                {
                    Message = "An unexpected error occurred while removing assignee.",
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

