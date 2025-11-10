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

        private readonly CaseService _caseService;

        public CasesController(CaseService caseService)
        {
            _caseService = caseService;
        }

       
        // ================================================================
        // POST: CreateCase
        // Description: Creates a new case  and link to existing crime reports
        // ================================================================
        [HttpPost("CreateCase")]
        [Authorize(Roles = "Admin,Investigator")]
        public async Task<IActionResult> CreateCase([FromBody] CaseCreateDto dto, CancellationToken ct)
        {
            try
            {
                if (dto == null)
                    return BadRequest("Invalid input data.");

                // Get username from JWT token
                var username = User.Identity?.Name;
                if (string.IsNullOrEmpty(username))
                    return Unauthorized("User identity not found.");

                // Example: Hardcoded ID (replace later with real user lookup)
                int createdByUserId = 1;

                var (caseId, caseNumber) = await _caseService.CreateAsync(createdByUserId, dto, ct);

                return CreatedAtAction(nameof(GetCaseById), new { id = caseId }, new
                {
                    Message = "Case created successfully.",
                    CaseId = caseId,
                    CaseNumber = caseNumber
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while creating the case.", Details = ex.Message });
            }
        }

        // ================================================================
        // GET: api/cases
        // Description: Return list of all cases with 100-char trimmed description
        // ================================================================
        [HttpGet("GetAllCases")]
        [Authorize(Roles = "Admin,Investigator,Officer")]
        public async Task<IActionResult> GetAllCases([FromQuery] string? q, CancellationToken ct)
        {
            try
            {
                var cases = await _caseService.GetAllAsync(q, ct);
                if (cases == null || !cases.Any())
                    return NotFound(new { Message = "No cases found." });

                return Ok(cases);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Failed to retrieve cases.", Details = ex.Message });
            }
        }

        // ================================================================
        // GET: api/cases/GetCaseById/{id}
        // Description: Retrieve case by ID (for Admin, Investigator, Officer)
        // ================================================================
        [HttpGet("GetCaseById")]
        [Authorize(Roles = "Admin,Investigator,Officer")]
        public async Task<IActionResult> GetCaseById(int id, CancellationToken ct)
        {
            try
            {
                var caseDetails = await _caseService.GetByIdAsync(id, ct);
                if (caseDetails == null)
                    return NotFound(new { Message = $"Case with ID {id} not found." });

                return Ok(caseDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Failed to get case details.", Details = ex.Message });
            }
        }

        // ================================================================
        // PUT: api/cases/{id}
        // Description: Update an existing case (Admin or Investigator)
        // ================================================================
        [HttpPut("UpdateCase/{id:int}")]
        [Authorize(Roles = "Admin,Investigator")]
        public async Task<IActionResult> UpdateCase(int id, [FromBody] CaseUpdateDto dto, CancellationToken ct)
        {
            try
            {
                if (dto == null)
                    return BadRequest(new { Message = "Invalid input data." });

                var updatedCase = await _caseService.UpdateAsync(id, dto, ct);

                if (updatedCase == null)
                    return NotFound(new { Message = $"Case with ID {id} not found." });

                return Ok(new
                {
                    Message = "Case updated successfully.",
                    UpdatedCase = updatedCase
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error occurred while updating case.", Details = ex.Message });
            }
        }




        // ================================================================
        // DELETE: api/cases/{id}
        // Description: Delete a case (Admin only)
        // ================================================================
        [HttpDelete("DeleteCase/{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCase(int id, CancellationToken ct)
        {
            try
            {
                var success = await _caseService.DeleteAsync(id, ct);
                if (!success)
                    return NotFound(new { Message = $"Case with ID {id} not found." });

                return Ok(new { Message = "Case deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error occurred while deleting case.", Details = ex.Message });
            }
        }
    }
}
