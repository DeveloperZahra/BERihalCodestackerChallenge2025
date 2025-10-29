// Repositories/Implementations/CaseRepository.cs
using BERihalCodestackerChallenge2025.Model;

namespace BERihalCodestackerChallenge2025.Repositories
{
    public interface ICaseRepository // Case repository interface extending generic repository for case-specific operations
    {
        Task<bool> ExistsByNumberAsync(string caseNumber, CancellationToken ct = default); // Check if a case exists by its case number
        Task<Case?> GetDetailsAsync(int id, CancellationToken ct = default); // Retrieve detailed information about a case by its ID
        Task LinkReportsAsync(int caseId, IEnumerable<int> reportIds, CancellationToken ct = default); // Link multiple reports to a case
        Task<IEnumerable<Case>> SearchAsync(string? q, CancellationToken ct = default); // Search cases by query string in name or description
    }
}