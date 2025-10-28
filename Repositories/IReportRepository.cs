using BERihalCodestackerChallenge2025.Model;

namespace BERihalCodestackerChallenge2025.Repositories
{
    public interface IReportRepository
    {
        Task<CrimeReport?> GetByTrackingCodeAsync(string trackingCode);
    }
}