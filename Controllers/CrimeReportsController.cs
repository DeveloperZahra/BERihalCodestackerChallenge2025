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
    public class CrimeReportsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public CrimeReportsController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // ================================================================
        // POST: api/crimereports/public
        // Description: Allow citizens to submit crime reports anonymously.
        // ================================================================
        [HttpPost("CreateCrimeReport")]
        [AllowAnonymous] // Citizens can submit reports without authentication
        public async Task<IActionResult> SubmitCrimeReport([FromBody] CrimeReportCreateDto dto)
        {
            // Map DTO to Model
            var report = _mapper.Map<CrimeReport>(dto);

            // Citizen reports anonymously (no user linked)
            report.ReportedByUserId = null;
            report.Status = ReportStatus.pending;

            report.ReportDateTime = DateTime.UtcNow;

            // Generate tracking code after save
            _context.CrimeReports.Add(report);
            await _context.SaveChangesAsync();

            report.TrackingCode = $"CR-{DateTime.UtcNow:yyyy}-{report.Id:D6}";
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Crime report submitted successfully.",
                reportId = report.Id,
                trackingCode = report.TrackingCode
            });
        }

        // ================================================================
        // GET: api/crimereports/track/{code}
        // Description: Citizens can track report status using tracking code.
        // ================================================================
        [HttpGet("TrackCrimeReport/{reportId}")]
        [AllowAnonymous]
        public async Task<IActionResult> TrackReportByCode(string code)
        {
            var report = await _context.CrimeReports
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.TrackingCode == code);

            if (report == null)
                return NotFound("No report found with this tracking code.");

            return Ok(new
            {
                reportId = report.Id,
                trackingCode = report.TrackingCode,
                status = report.Status,
                area = report.AreaCity,
                description = report.Description,
                reportedAt = report.ReportDateTime
            });
        }

        // ================================================================
        // GET: api/crimereports
        // Description: Admin / Investigator can view all reports.
        // ================================================================
        [HttpGet("GetAllCrimeReports")]
        [Authorize(Roles = "Admin, Investigator")]
        public async Task<ActionResult<IEnumerable<CrimeReportStatusDto>>> GetAllReports()
        {
            var reports = await _context.CrimeReports
                .Include(r => r.ReportedByUser)
                .AsNoTracking()
                .OrderByDescending(r => r.ReportDateTime)
                .ToListAsync();

            return Ok(_mapper.Map<IEnumerable<CrimeReportStatusDto>>(reports));
        }

        // ================================================================
        // GET: api/crimereports/{id}
        // Description: View specific report by ID
        // ================================================================
        [HttpGet("GetCrimeReportById/{id:int}")]
        [Authorize(Roles = "Admin, Investigator")]
        public async Task<ActionResult<CrimeReportStatusDto>> GetReportById(int id)
        {
            var report = await _context.CrimeReports
                .Include(r => r.ReportedByUser)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (report == null)
                return NotFound("Report not found.");

            return Ok(_mapper.Map<CrimeReportStatusDto>(report));
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

            return Ok(new { message = $"Status updated to '{status}'." });
        }
    }
}
