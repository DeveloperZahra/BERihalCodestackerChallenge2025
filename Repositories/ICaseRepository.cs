// Repositories/Implementations/CaseRepository.cs
using BERihalCodestackerChallenge2025.Model;

namespace BERihalCodestackerChallenge2025.Repositories
{
    public interface ICaseRepository
    {
        Task<bool> ExistsByNumberAsync(string caseNumber, CancellationToken ct = default);

        
        Task<Case?> GetDetailsAsync(int id, CancellationToken ct = default);
        Task LinkReportsAsync(int caseId, IEnumerable<int> reportIds, CancellationToken ct = default);
        Task<IEnumerable<Case>> SearchAsync(string? q, CancellationToken ct = default);
    }
}