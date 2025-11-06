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
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CrimeReportsController : ControllerBase
    {
        private readonly ReportService _reportService;

        public CrimeReportsController(ReportService reportService)
        {
            _reportService = reportService;
        }

        // ================================================================
        // POST: api/crimereports/public
        // Description: Allow citizens to submit crime reports anonymously.
        // ================================================================
        [AllowAnonymous] // Citizens can submit reports without authentication
        [HttpPost("CreateCrimeReport")]

        public async Task<IActionResult> SubmitCrimeReport([FromBody] CrimeReportCreateDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid report data.");

            var createdReport = await _reportService.SubmitAsync(dto);

            return Ok(new
            {
                message = "Crime report submitted successfully.",
                reportId = createdReport.Id,
                trackingCode = createdReport.TrackingCode,
                status = createdReport.Status
            });
        }

        // ================================================================
        // GET: api/crimereports/track/{code}
        // Description: Citizens can track report status using tracking code.
        // ================================================================
        [HttpGet("TrackCrimeReport/{code}")]
        [AllowAnonymous]
        public async Task<IActionResult> TrackReportByCode(string code)
        {
            var report = await _reportService.GetStatusAsync(code);
            if (report == null)
                return NotFound("No report found with this tracking code.");

            return Ok(report);
        }

        // ================================================================
        // GET: api/crimereports
        // Description: Admin / Investigator can view all reports.
        // ================================================================
        [HttpGet("GetAllCrimeReports")]
        [Authorize(Roles = "Admin, Investigator")]
        public IActionResult GetAllReports()
        {
         
            return BadRequest("Operation not supported yet.");
        }

        // ================================================================
        // GET: api/crimereports/{id}
        // Description: View specific report by ID
        // ================================================================
        [HttpGet("GetCrimeReportById/{id:int}")]
        [Authorize(Roles = "Admin, Investigator")]
        public async Task<IActionResult> GetCrimeReportById(int id)
        {
            var report = await _reportService.GetStatusAsync(id.ToString());
            if (report == null)
                return NotFound("Report not found.");

            return Ok(report);
        }

        // ================================================================
        // PUT: api/crimereports/{id}/status
        // Description: Admin / Investigator can update report status
        // ================================================================
        [HttpPut("UpdateCrimeReportStatus/{id:int}")]
        [Authorize(Roles = "Admin, Investigator")]
        public async Task<IActionResult> UpdateReportStatus(int id, [FromBody] string status, CancellationToken ct)
                      => await _reportService.UpdateStatusAsync(id, status, ct) ? Ok() : NotFound();
    }
}
