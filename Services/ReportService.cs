// Services/ReportService.cs
using BERihalCodestackerChallenge2025.DTOs;
using BERihalCodestackerChallenge2025.Model;
using BERihalCodestackerChallenge2025.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BERihalCodestackerChallenge2025.Services
{
    public class ReportService : IReportService // Service for handling crime report submissions and status retrieval
    {
        private readonly IUnitOfWork _uow; // Unit of Work for managing repositories and transactions
        private readonly IGenericRepository<CrimeReport> _reports; // Repository for crime reports
        private readonly IGenericRepository<User> _users;  // Repository for users       

        public ReportService( 
            IUnitOfWork uow,
            IGenericRepository<CrimeReport> reportsRepo, 
            IGenericRepository<User> usersRepo) 
        {
            _uow = uow; 
            _reports = reportsRepo; 
            _users = usersRepo; 
        }


        public async Task<CrimeReportStatusDto> SubmitAsync(CrimeReportCreateDto dto, CancellationToken ct = default) // Submit a new crime report
        {
            
            User? reportedBy = null; // Variable to hold the reporting user, if any
            if (dto.ReportedByUserId is not null) // If a reporting user ID is provided
            {
                reportedBy = await _users.GetByIdAsync(dto.ReportedByUserId.Value, ct) // Retrieve the user from the database
                              ?? throw new InvalidOperationException("reported_by must be Admin or Investigator, or null for Citizen."); // Throw if user not found

                if (reportedBy.Role != Role.Admin && reportedBy.Role != Role.Investigator) 
                    throw new InvalidOperationException("reported_by must be Admin or Investigator, or null for Citizen."); // Validate the user's role
            }

            var tracking = $"CR-{DateTime.UtcNow:yyyy}-{Random.Shared.Next(1, 999999):000000}"; // Generate a unique tracking code for the report

            var entity = new CrimeReport // Create a new CrimeReport entity
            {
                Title = dto.Title, // Short title of the crime report
                Description = dto.Description, // Detailed description of the crime
                AreaCity = dto.AreaCity, // City where the crime occurred
                Latitude = dto.Latitude, // Geographic coordinates of the crime location
                Longitude = dto.Longitude, // Geographic coordinates of the crime location
                Status = ReportStatus.pending, // Initial status of the report
                TrackingCode = tracking, // Set the generated tracking code
                ReportedByUserId = dto.ReportedByUserId // Set the reporting user ID
            };

            await _reports.AddAsync(entity, ct);  // Add the new report to the repository
            await _uow.SaveChangesAsync(ct);     // Save changes to the database 

            return new CrimeReportStatusDto // Return the status of the submitted report
            {
                Id = entity.Id,
                TrackingCode = entity.TrackingCode, //  Tracking code of the report
                Status = entity.Status.ToString(), // Current status of the report
                AreaCity = entity.AreaCity, // City where the crime occurred
                ReportedBy = reportedBy?.Username ?? "Citizen", // Username of the reporting user or "Citizen" if null
                ReportDateTime = entity.ReportDateTime // Timestamp of when the report was created
            };
        }


        public async Task<CrimeReportStatusDto?> GetStatusAsync(string idOrTracking, CancellationToken ct = default) // Retrieve the status of a crime report by ID or tracking code
        {
            CrimeReport? report; // Variable to hold the retrieved report

            if (int.TryParse(idOrTracking, out var id)) // If the input can be parsed as an integer ID
            {
                
                report = await _reports.GetByIdAsync(id, ct); // Retrieve the report by ID
            }
            else
            {
               
                report = await _uow.Reports.GetByTrackingCodeAsync(idOrTracking, ct); // Retrieve the report by tracking code
            }

            if (report is null) return null; // Return null if the report is not found

            var reportedBy = report.ReportedByUserId is null
                ? "Citizen"
                : (await _users.GetByIdAsync(report.ReportedByUserId.Value, ct))?.Username; // Retrieve the reporting user from the database

            return new CrimeReportStatusDto // Return the status of the retrieved report
            {
                Id = report.Id,
                TrackingCode = report.TrackingCode, // Tracking code of the report
                Status = report.Status.ToString(), // Current status of the report
                AreaCity = report.AreaCity, // City where the crime occurred
                ReportedBy = reportedBy, // Username of the reporting user or "Citizen"
                ReportDateTime = report.ReportDateTime // Timestamp of when the report was created
            };
        }
    }
}




//using BERihalCodestackerChallenge2025.DTOs;
//using BERihalCodestackerChallenge2025.Model;
//using BERihalCodestackerChallenge2025.Repositories;
//using Microsoft.EntityFrameworkCore;

//namespace BERihalCodestackerChallenge2025.Services
//{
//    public class ReportService : IReportService
//    // Service for handling crime report submissions and status retrieval
//    {
//        private readonly IUnitOfWork _uow; // Unit of Work for managing repositories and transactions



//        private readonly IGenericRepository<User> _users;

//        private readonly IGenericRepository<CrimeReport> _reports;
//        public ReportService(IUnitOfWork uow) => _uow = uow; // Constructor accepting the Unit of Work

//        public async Task<CrimeReportStatusDto> SubmitAsync(CrimeReportCreateDto dto, CancellationToken ct = default) // Submit a new crime report
//        {
//            User? reportedBy = null; // Variable to hold the reporting user, if any
//            if (dto.ReportedByUserId is not null) // If a reporting user ID is provided
//            {
//                reportedBy = await _uow.Users.GetByIdAsync(dto.ReportedByUserId.Value, ct); // Retrieve the user from the database

//                if (reportedBy is null || (reportedBy.Role != Role.Admin && reportedBy.Role != Role.Investigator)) // Validate the user's role
//                    throw new InvalidOperationException("reported_by must be Admin or Investigator, or null for Citizen."); // Throw an exception if the user is invalid
//            }

//            var tracking = $"CR-{DateTime.UtcNow:yyyy}-{Random.Shared.Next(1, 999999):000000}"; // Generate a unique tracking code for the report
//            var entity = new CrimeReport // Create a new CrimeReport entity
//            {
//                Title = dto.Title, // Short title of the crime report
//                Description = dto.Description, // Detailed description of the crime
//                AreaCity = dto.AreaCity, // City where the crime occurred
//                Latitude = dto.Latitude, // Geographic coordinates of the crime location
//                Longitude = dto.Longitude, // Geographic coordinates of the crime location
//                Status = ReportStatus.pending, // Initial status of the report
//                TrackingCode = tracking, // Set the generated tracking code
//                ReportedByUserId = dto.ReportedByUserId // Set the reporting user ID
//            };
//            await _reports.AddAsync(entity, ct);// Add the new report to the repository
//            await _uow.SaveChangesAsync(ct); // Save changes to the database

//            return new CrimeReportStatusDto // Return the status of the submitted report
//            {
//                Id = entity.Id, // Unique identifier of the report
//                TrackingCode = entity.TrackingCode, //  Tracking code of the report
//                Status = entity.Status.ToString(), // Current status of the report
//                AreaCity = entity.AreaCity, // City where the crime occurred
//                ReportedBy = reportedBy?.Username ?? "Citizen", // Username of the reporting user or "Citizen" if null
//                ReportDateTime = entity.ReportDateTime // Timestamp of when the report was created
//            };
//        }

//        public async Task<CrimeReportStatusDto?> GetStatusAsync(string idOrTracking, CancellationToken ct = default) // Retrieve the status of a crime report by ID or tracking code
//        {
//            CrimeReport? report = null; // Variable to hold the retrieved report

//            if (int.TryParse(idOrTracking, out var id)) // If the input can be parsed as an integer ID
//                report = await _uow.Reports.GetByIdAsync(id, ct); // Retrieve the report by ID
//            else
//                report = await _uow.Reports.GetByTrackingCodeAsync(idOrTracking, ct); // Retrieve the report by tracking code

//            if (report is null) return null; // Return null if the report is not found

//            var reportedBy = report.ReportedByUserId is null ? "Citizen" // Get the reporting user's username or "Citizen" if null
//                : (await _uow.Users.GetByIdAsync(report.ReportedByUserId.Value, ct))?.Username; // Retrieve the reporting user from the database

//            return new CrimeReportStatusDto // Return the status of the retrieved report
//            {
//                Id = report.Id, // Unique identifier of the report
//                TrackingCode = report.TrackingCode, // Tracking code of the report
//                Status = report.Status.ToString(), // Current status of the report
//                AreaCity = report.AreaCity, // City where the crime occurred
//                ReportedBy = reportedBy, // Username of the reporting user or "Citizen"
//                ReportDateTime = report.ReportDateTime // Timestamp of when the report was created
//            }; // Return the populated DTO
//        }
//    }
//}
