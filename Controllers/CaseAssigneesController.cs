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

        // ================================================================
        // POST: api/caseassignees
        // Description: Assign an officer/investigator to a case
        // ================================================================
        [HttpPost]
        public async Task<IActionResult> AssignUserToCase([FromBody] CaseAssigneeCreateDto dto)
        {
            // ✅ Check if case exists
            var caseEntity = await _context.Cases.FindAsync(dto.CaseId);
            if (caseEntity == null)
                return NotFound("Case not found.");

            // ✅ Check if user exists
            var user = await _context.Users.FindAsync(dto.UserId);
            if (user == null)
                return NotFound("User not found.");

            // 🚫 Prevent duplicate assignment
            bool alreadyAssigned = await _context.CaseAssignees
                .AnyAsync(a => a.CaseId == dto.CaseId && a.UserId == dto.UserId);

            if (alreadyAssigned)
                return Conflict("User already assigned to this case.");

            // 🧩 Map DTO → Entity
            var assignee = _mapper.Map<CaseAssignee>(dto);
            assignee.AssignedAt = DateTime.UtcNow;
            assignee.ProgressStatus = CaseStatus.pending; // default value

            _context.CaseAssignees.Add(assignee);
            await _context.SaveChangesAsync();

            // ✅ Return success response
            return Ok($"User '{user.Username}' assigned successfully to case '{caseEntity.Name}'.");
        }


        // ================================================================
        // PUT: api/caseassignees/progress/{assigneeId}
        // Description: Update progress status (Officer only)
        // ================================================================
        [Authorize(Roles = "Officer,Investigator,Admin")]
        [HttpPut("progress/{assigneeId:int}")]
        public async Task<IActionResult> UpdateProgressStatus(int assigneeId, [FromBody] CaseProgressUpdateDto dto)
        {
            var assignee = await _context.CaseAssignees
                .Include(a => a.Case)
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.Id == assigneeId);

            if (assignee == null)
                return NotFound("Assignee not found.");

            // 🧠 Update allowed field
            assignee.ProgressStatus = Enum.Parse<CaseStatus>(dto.ProgressStatus, true);

            await _context.SaveChangesAsync();

            return Ok($"Progress updated for user {assignee.User.Username} on case {assignee.Case.Name}.");
        }

        // ================================================================
        // DELETE: api/caseassignees/{id}
        // Description: Remove an assignee from a case (Admin/Investigator)
        // ================================================================
        [HttpDelete("{id:int}")]
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
            return userIndex >= caseIndex; // ✅ User must have >= clearance
        }
    }
}
