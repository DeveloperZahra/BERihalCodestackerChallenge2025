using BERihalCodestackerChallenge2025.DTOs;

namespace BERihalCodestackerChallenge2025.Services
{
    public interface IReportService // Report service interface for handling crime report operations
    {
        Task<CrimeReportStatusDto?> GetStatusAsync(string idOrTracking, CancellationToken ct = default); // Retrieve the status of a crime report by its ID or tracking code
        Task<CrimeReportStatusDto> SubmitAsync(CrimeReportCreateDto dto, CancellationToken ct = default); // Submit a new crime report and return its status
    }
}