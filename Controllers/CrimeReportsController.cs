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
        private readonly IReportService _reportService;

        public CrimeReportsController(IReportService reportService)
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
            try
            {
                //  1. Validate input 
                if (dto == null)
                    return BadRequest(new { Message = "Invalid report data." });

                //  2. Call the service layer 
                var createdReport = await _reportService.SubmitAsync(dto);

                //  3. Return success response 
                return Ok(new
                {
                    Message = "Crime report submitted successfully.",
                    ReportId = createdReport.Id,
                    TrackingCode = createdReport.TrackingCode,
                    Status = createdReport.Status
                });
            }
            catch (ArgumentException ex)
            {
                //  Error in data entered by the user
                return BadRequest(new
                {
                    Message = ex.Message,
                    Details = "Please verify your input fields."
                });
            }
            catch (KeyNotFoundException)
            {
                //  Error in finding required data within the system
                return NotFound(new
                {
                    Message = "Related record not found.",
                    Details = "Some referenced entity does not exist."
                });
            }
            catch (InvalidOperationException ex)
            {
                // A logical error during processing (such as a condition that cannot be executed)
                return Conflict(new
                {
                    Message = "Operation could not be completed.",
                    Details = ex.Message
                });
            }
            catch (Exception ex)
            {
                //  (Unexpected Error)
                return StatusCode(500, new
                {
                    Message = "An unexpected error occurred while submitting the crime report.",
                    Details = ex.Message
                });
            }
        }


        // ================================================================
        // GET: TrackCrimeReport/{code}
        // Description: Tracks a crime report by its unique tracking code
        // ================================================================
        [HttpGet("TrackCrimeReport")]
        [AllowAnonymous]
        public async Task<IActionResult> TrackReportByCode(string code, CancellationToken ct)
        {
            try
            {
                //  1. Validate input
                if (string.IsNullOrWhiteSpace(code))
                    return BadRequest(new { Message = "Tracking code is required." });

                //  2. Attempt to retrieve report
                var report = await _reportService.GetStatusAsync(code, ct);

                //  3. Handle not found
                if (report == null)
                    return NotFound(new
                    {
                        Message = "No report found with this tracking code.",
                        ProvidedCode = code
                    });

                //  4. Return success result
                return Ok(new
                {
                    Message = "Report found successfully.",
                    Report = report
                });
            }
            catch (ArgumentException ex)
            {
                //  Handle invalid argument or format errors
                return BadRequest(new
                {
                    Message = "Invalid tracking code format.",
                    Details = ex.Message
                });
            }
            catch (OperationCanceledException)
            {
                //  Handle cancellation (e.g., client disconnected)
                return StatusCode(499, "Request was cancelled by the client.");
            }
            catch (KeyNotFoundException ex)
            {
                //  Handle missing data scenario from the service layer
                return NotFound(new
                {
                    Message = "No matching report found in the system.",
                    Details = ex.Message
                });
            }
            catch (Exception ex)
            {
                //  Catch all unexpected errors
                var baseEx = ex.GetBaseException();
                return StatusCode(500, new
                {
                    Message = "An unexpected error occurred while tracking the report.",
                    Error = baseEx.Message
                });
            }
        }


        // ================================================================
        // GET: api/crimereports/GetAllCrimeReports
        // Description: Allows Admin and Investigator to view all reports with full error handling
        // ================================================================
        [HttpGet("GetAllCrimeReports")]
        [Authorize(Roles = "Admin, Investigator")]
        public async Task<IActionResult> GetAllReports(CancellationToken ct)
        {
            try
            {
                //  1. Attempt to retrieve all reports from service
                var reports = await _reportService.GetAllAsync(ct);

                //  2. Handle if no reports found
                if (reports == null || !reports.Any())
                    return NotFound(new
                    {
                        Message = "No crime reports found in the system."
                    });

                //  3. Return success result
                return Ok(new
                {
                    Message = "Crime reports retrieved successfully.",
                    Count = reports.Count(),
                    Reports = reports
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                //  If user is authenticated but lacks permissions
                return Forbid(ex.Message);
            }
            catch (OperationCanceledException)
            {
                //  If request was cancelled by client
                return StatusCode(499, "Request was cancelled by the client.");
            }
            catch (ArgumentException ex)
            {
                //  Handle invalid request parameters or service-level argument errors
                return BadRequest(new
                {
                    Message = "Invalid request parameters.",
                    Details = ex.Message
                });
            }
            catch (Exception ex)
            {
                //  Catch all unexpected errors
                var baseEx = ex.GetBaseException();
                return StatusCode(500, new
                {
                    Message = "An unexpected error occurred while retrieving crime reports.",
                    Error = baseEx.Message
                });
            }
        }


        // ================================================================
        // GET: api/crimereports/{id}
        // Description: View specific report by ID
        // ================================================================
        [HttpGet("GetCrimeReportById")]
        [Authorize(Roles = "Admin, Investigator")]
        public async Task<IActionResult> GetCrimeReportById(int id, CancellationToken ct)
        {
            try
            {
                //  1. Validate input 
                if (id <= 0)
                    return BadRequest(new { Message = "Invalid report ID. It must be greater than zero." });

                //  2. Retrieve report by ID 
                var report = await _reportService.GetStatusAsync(id.ToString(), ct);

                //  3. Handle not found 
                if (report == null)
                    return NotFound(new { Message = $"Crime report with ID {id} was not found." });

                //  4. Return success result 
                return Ok(new
                {
                    Message = "Crime report retrieved successfully.",
                    Report = report
                });
            }
            catch (KeyNotFoundException ex)
            {
                //  5. Handle specific 'not found' exception
                return NotFound(new
                {
                    Message = "No crime report found.",
                    Details = ex.Message
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                //  6. User not authorized 
                return Forbid(ex.Message);
            }
            catch (ArgumentException ex)
            {
                //  7. Invalid input data
                return BadRequest(new
                {
                    Message = "Invalid argument or input value.",
                    Details = ex.Message
                });
            }
            catch (Exception ex)
            {
                //  8. Unexpected error 
                return StatusCode(500, new
                {
                    Message = "An unexpected error occurred while retrieving the crime report.",
                    Details = ex.Message
                });
            }
        }


        // ================================================================
        // PUT: api/crimereports/{id}/status
        // Description: Admin / Investigator can update report status
        // ================================================================
        [HttpPut("UpdateCrimeReportStatus")]
        [Authorize(Roles = "Admin, Investigator")]
        public async Task<IActionResult> UpdateReportStatus(int id, [FromBody] string status, CancellationToken ct)
        {
            try
            {
                //  1. Validate input 
                if (id <= 0)
                    return BadRequest(new { Message = "Invalid report ID. It must be greater than zero." });

                if (string.IsNullOrWhiteSpace(status))
                    return BadRequest(new { Message = "Status value cannot be empty." });

                //  2. Attempt to update the report status
                var success = await _reportService.UpdateStatusAsync(id, status, ct);

                //  3. Handle 'not found' case
                if (!success)
                    return NotFound(new { Message = $"Crime report with ID {id} was not found." });

                //  4. Return success response
                return Ok(new
                {
                    Message = "Crime report status updated successfully.",
                    ReportId = id,
                    NewStatus = status
                });
            }
            catch (ArgumentException ex)
            {
                //  5. Invalid status value or input
                return BadRequest(new
                {
                    Message = "Invalid status value.",
                    Details = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                //  6. Business logic violation (for example: cannot move from 'closed' to 'pending')
                return Conflict(new
                {
                    Message = "Operation not allowed.",
                    Details = ex.Message
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                //  7. User not authorized for this action
                return Forbid(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                //  8. Report not found
                return NotFound(new
                {
                    Message = "No matching report found.",
                    Details = ex.Message
                });
            }
            catch (Exception ex)
            {
                //  9. Unexpected error
                return StatusCode(500, new
                {
                    Message = "An unexpected error occurred while updating the crime report status.",
                    Details = ex.Message
                });
            }
        }

    }
}
