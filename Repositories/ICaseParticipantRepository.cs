// Repositories/Implementations/CaseParticipantRepository.cs
using BERihalCodestackerChallenge2025.Model;

namespace BERihalCodestackerChallenge2025.Repositories
{
    public interface ICaseParticipantRepository // Case participant repository interface extending generic repository for case participant-specific operations
    {
        Task<IEnumerable<CaseParticipant>> GetByCaseAndRoleAsync(int caseId, ParticipantRole role, CancellationToken ct = default); // Retrieve participants for a specific case and role
    }
}