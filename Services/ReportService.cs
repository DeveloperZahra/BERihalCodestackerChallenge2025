using BERihalCodestackerChallenge2025.DTOs;
using BERihalCodestackerChallenge2025.Model;
using BERihalCodestackerChallenge2025.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BERihalCodestackerChallenge2025.Services
{
    public class ReportService 
    {
        private readonly IUnitOfWork _uow;
        public ReportService(IUnitOfWork uow) => _uow = uow;

        public async Task<CrimeReportStatusDto> SubmitAsync(CrimeReportCreateDto dto, CancellationToken ct = default)
        {
            User? reportedBy = null;
            if (dto.ReportedByUserId is not null)
            {
                reportedBy = await _uow.Users.GetByIdAsync(dto.ReportedByUserId.Value, ct);
                if (reportedBy is null || (reportedBy.Role != Role.Admin && reportedBy.Role != Role.Investigator))
                    throw new InvalidOperationException("reported_by must be Admin or Investigator, or null for Citizen.");
            }

            var tracking = $"CR-{DateTime.UtcNow:yyyy}-{Random.Shared.Next(1, 999999):000000}";
            var entity = new CrimeReport
            {
                Title = dto.Title,
                Description = dto.Description,
                AreaCity = dto.AreaCity,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                Status = ReportStatus.pending,
                TrackingCode = tracking,
                ReportedByUserId = dto.ReportedByUserId
            };
            await _uow.Reports.AddAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);

            return new CrimeReportStatusDto
            {
                Id = entity.Id,
                TrackingCode = entity.TrackingCode,
                Status = entity.Status.ToString(),
                AreaCity = entity.AreaCity,
                ReportedBy = reportedBy?.Username ?? "Citizen",
                ReportDateTime = entity.ReportDateTime
            };
        }

        public async Task<CrimeReportStatusDto?> GetStatusAsync(string idOrTracking, CancellationToken ct = default)
        {
            CrimeReport? report = null;

            if (int.TryParse(idOrTracking, out var id))
                report = await _uow.Reports.GetByIdAsync(id, ct);
            else
                report = await _uow.Reports.GetByTrackingCodeAsync(idOrTracking, ct);

            if (report is null) return null;

            var reportedBy = report.ReportedByUserId is null ? "Citizen"
                : (await _uow.Users.GetByIdAsync(report.ReportedByUserId.Value, ct))?.Username;

            return new CrimeReportStatusDto
            {
                Id = report.Id,
                TrackingCode = report.TrackingCode,
                Status = report.Status.ToString(),
                AreaCity = report.AreaCity,
                ReportedBy = reportedBy,
                ReportDateTime = report.ReportDateTime
            };
        }
    }
}
