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
    public class CrimeReportsController : ControllerBase
    {
        {
        }

        // ================================================================
        // POST: api/crimereports/public
        // Description: Allow citizens to submit crime reports anonymously.
        // ================================================================
        [AllowAnonymous] // Citizens can submit reports without authentication
        [HttpPost("CreateCrimeReport")]

        public async Task<IActionResult> SubmitCrimeReport([FromBody] CrimeReportCreateDto dto)
        {


            report.ReportDateTime = DateTime.UtcNow;

            // Generate tracking code after save
            _context.CrimeReports.Add(report);
            await _context.SaveChangesAsync();

            report.TrackingCode = $"CR-{DateTime.UtcNow:yyyy}-{report.Id:D6}";
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Crime report submitted successfully.",
            });
        }

        // ================================================================
        // GET: api/crimereports/track/{code}
        // Description: Citizens can track report status using tracking code.
        // ================================================================
        [AllowAnonymous]
        public async Task<IActionResult> TrackReportByCode(string code)
        {
            if (report == null)
                return NotFound("No report found with this tracking code.");

        }


        // ================================================================
        // GET: api/crimereports
        // Description: Admin / Investigator can view all reports.
        // ================================================================
        [HttpGet("GetAllCrimeReports")]
        [Authorize(Roles = "Admin, Investigator")]
        {
            var reports = await _context.CrimeReports
                .Include(r => r.ReportedByUser)
                .AsNoTracking()
                .OrderByDescending(r => r.ReportDateTime)
                .ToListAsync();

        }

        // ================================================================
        // GET: api/crimereports/{id}
        // Description: View specific report by ID
        // ================================================================
        [HttpGet("GetCrimeReportById/{id:int}")]
        [Authorize(Roles = "Admin, Investigator")]
        {
            if (report == null)
                return NotFound("Report not found.");

        }

        // ================================================================
        // PUT: api/crimereports/{id}/status
        // Description: Admin / Investigator can update report status
        // ================================================================
        [HttpPut("UpdateCrimeReportStatus/{id:int}")]
        [Authorize(Roles = "Admin, Investigator")]
        public async Task<IActionResult> UpdateReportStatus(int id, [FromBody] string status)
        {
            var report = await _context.CrimeReports.FindAsync(id);
            if (report == null)
                return NotFound("Report not found.");

            report.Status = ReportStatus.resolved;
            
            await _context.SaveChangesAsync();

        }
    }
}
