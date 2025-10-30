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
        [HttpPost("assign")]
        public IActionResult AssignUserToCase([FromBody] CaseAssigneeCreateDto dto)
        {
            //  Step 1: Validate input
            if (dto == null || dto.CaseId <= 0 || dto.UserId <= 0)
                return BadRequest("Invalid case or user information.");

            //  Step 2: Check if case and user exist
            var caseEntity = _context.Cases.FirstOrDefault(c => c.CaseId == dto.CaseId);
            var userEntity = _context.Users.FirstOrDefault(u => u.UserId == dto.UserId);

            if (caseEntity == null)
                return NotFound($"Case with ID {dto.CaseId} not found.");
            if (userEntity == null)
                return NotFound($"User with ID {dto.UserId} not found.");

            //  Step 3: Check if already assigned
            bool alreadyAssigned = _context.CaseAssignees.Any(a => a.CaseId == dto.CaseId && a.UserId == dto.UserId);
            if (alreadyAssigned)
                return Conflict("User is already assigned to this case.");

            //  Step 4: Create new assignment object
            var assignment = new CaseAssignee
            {
                CaseId = dto.CaseId,
                UserId = dto.UserId,
                AssignedAt = DateTime.UtcNow,
                ClearanceLevel = dto.ClearanceLevel
            };

            //  Step 5: Add to database
            _context.CaseAssignees.Add(assignment);
            _context.SaveChanges();

            //  Step 6: Return success response (⚠️ notice variable inside same scope)
            return Ok(new
            {
                Message = "User assigned successfully to case.",
                CaseAssigneeId = assignment.CaseAssigneeId, 
                assignment.CaseId,
                assignment.UserId,
                assignment.ClearanceLevel
            });
        }

        // ============================================================
        //  Get all assignees for a specific case
        // ============================================================
        [HttpGet("GetAllAssignees")]
        public IActionResult GetAssigneesByCase(int caseId)
        {
            var caseExists = _context.Cases.Any(c => c.CaseId == caseId);
            if (!caseExists)
                return NotFound($"Case with ID {caseId} not found.");

            var assignees = _context.CaseAssignees
                .Where(a => a.CaseId == caseId)
                .Select(a => new
                {
                    a.CaseAssigneeId,
                    a.CaseId,
                    a.UserId,
                    a.AssignedAt,
                    a.ClearanceLevel
                })
                .ToList();

            return Ok(assignees);
        }
    
       // ================================================================
       // Description: Update progress status (Officer only)
      // ================================================================
        [Authorize(Roles = "Officer,Investigator,Admin")]
            [HttpPut("UpdateCaseProgress/{assigneeId:int}")]
            public async Task<IActionResult> UpdateProgressStatus(int assigneeId, [FromBody] CaseProgressUpdateDto dto)
            {
                var assignee = await _context.CaseAssignees
                    .Include(a => a.Case)
                    .Include(a => a.User)
                    .FirstOrDefaultAsync(a => a.CaseAssigneeId == assigneeId);

                if (assignee == null)
                    return NotFound("Assignee not found.");

                // Update allowed field
                assignee.ProgressStatus = Enum.Parse<CaseStatus>(dto.ProgressStatus, true);

                await _context.SaveChangesAsync();

                return Ok($"Progress updated for user {assignee.User.Username} on case {assignee.Case.Name}.");
            }

            // ================================================================
            // DELETE: api/caseassignees/{id}
            // Description: Remove an assignee from a case (Admin/Investigator)
            // ================================================================
            [HttpDelete("DeleteAssignee/{id:int}")]
            public async Task<IActionResult> RemoveAssignee(int id)
            {
                var assignee = await _context.CaseAssignees.FindAsync(id);
                if (assignee == null)
                    return NotFound("Assignee not found.");

                _context.CaseAssignees.Remove(assignee);
                await _context.SaveChangesAsync();

                return Ok($"Assignee with ID {id} removed successfully.");
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

