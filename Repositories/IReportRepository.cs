using BERihalCodestackerChallenge2025.Model;

namespace BERihalCodestackerChallenge2025.Repositories
{
    public interface IReportRepository // Report repository interface for crime report-specific operations
    {
        Task<CrimeReport?> GetByTrackingCodeAsync(string trackingCode);
    }
}