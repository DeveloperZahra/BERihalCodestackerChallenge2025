// Repositories/Implementations/EvidenceRepository.cs
using BERihalCodestackerChallenge2025.Model;

namespace BERihalCodestackerChallenge2025.Repositories
{
    public interface IEvidenceRepository // Evidence repository interface extending generic repository for evidence-specific operations
    {
        Task<IEnumerable<Evidence>> GetByCaseAsync(int caseId, bool includeSoftDeleted = false, CancellationToken ct = default); // Retrieve all evidences for a specific case, with an option to include soft-deleted records
        Task<Evidence?> GetWithUserAsync(int id, CancellationToken ct = default); // Retrieve evidence by ID including the user who added it
        Task HardDeleteAsync(Evidence entity, int actedByUserId, string? details = null, CancellationToken ct = default); // Hard delete an evidence record and log the action
        Task SoftDeleteAsync(Evidence entity, int actedByUserId, string? details = null, CancellationToken ct = default); // Soft delete an evidence record and log the action
    }
}