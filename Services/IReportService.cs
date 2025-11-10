// Services/ReportService.cs
using BERihalCodestackerChallenge2025.DTOs;

namespace BERihalCodestackerChallenge2025.Services
{
    public interface IReportService
    {
        Task<IEnumerable<CrimeReportListDto>> GetAllAsync(CancellationToken ct = default);
        Task<CrimeReportStatusDto?> GetStatusAsync(string idOrTracking, CancellationToken ct = default);
        Task<CrimeReportStatusDto> SubmitAsync(CrimeReportCreateDto dto, CancellationToken ct = default);
        Task<bool> UpdateStatusAsync(int id, string status, CancellationToken ct);
    }
}