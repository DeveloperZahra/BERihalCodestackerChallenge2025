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
        private readonly CaseService _caseService;

        public CasesController(CaseService caseService)
        {
            _caseService = caseService;
        }

        // ================================================================
        // POST: api/cases
        // Description: Create a new case and link to existing crime reports
        // ================================================================
        [HttpPost("CreateCase")]
        [Authorize(Roles = "Admin,Investigator")]
        public async Task<IActionResult> CreateCase([FromBody] CaseCreateDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid input data.");

            
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return Unauthorized("User identity not found.");

          
            int createdByUserId = 1; 

            var (caseId, caseNumber) = await _caseService.CreateAsync(createdByUserId, dto);

            return CreatedAtAction(nameof(GetCaseById),
                new { id = caseId },
                new
                {
                    Message = "Case created successfully.",
                    CaseId = caseId,
                    CaseNumber = caseNumber
                });
        }

        // ================================================================
        // GET: api/cases/GetCaseById/{id}
        // Description: Retrieve case by ID (for Admin, Investigator, Officer)
        // ================================================================
        [HttpGet("GetCaseById/{id:int}")]
        [Authorize(Roles = "Admin,Investigator,Officer")]
        public async Task<IActionResult> GetCaseById(int id)
        {
            
            return BadRequest("Get by ID is not yet implemented in CaseService.");
        }

        // ================================================================
        // PUT: api/cases/{id}
        // Description: Update an existing case (Admin or Investigator)
        // ================================================================
        [HttpPut("UpdateCase/{id:int}")]
        [Authorize(Roles = "Admin,Investigator")]
        public async Task<IActionResult> UpdateCase(int id, [FromBody] CaseUpdateDto dto)
        {
           
            return BadRequest("Update case feature not implemented yet in CaseService.");
        }


        // ================================================================
        // GET: api/cases
        // Description: Return list of all cases with 100-char trimmed description
        // ================================================================
        [HttpGet("GetAllCases")]
        [Authorize(Roles = "Admin,Investigator,Officer")]
        public async Task<IActionResult> GetAllCases()
        {
            
            return BadRequest("Get all cases not implemented yet in CaseService.");
        }


        // ================================================================
        // DELETE: api/cases/{id}
        // Description: Delete a case (Admin only)
        // ================================================================
        [HttpDelete("DeleteCase/{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCase(int id)
        {
            
            return BadRequest("Delete case feature not implemented yet in CaseService.");
        }
    }
}
