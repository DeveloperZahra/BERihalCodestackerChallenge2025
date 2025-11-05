using BERihalCodestackerChallenge2025.DTOs;

namespace BERihalCodestackerChallenge2025.Services
{
    public interface IEvidenceService
    {
        Task<string> ConfirmHardDeleteAsync(int id, int actedByUserId, string yesNo, CancellationToken ct = default);
        Task<int> CreateTextAsync(int caseId, int addedByUserId, EvidenceCreateDto dto, CancellationToken ct = default);
        Task<bool> FinalizeHardDeleteAsync(int id, int actedByUserId, string command, CancellationToken ct = default);
        Task<EvidenceReadDto?> GetAsync(int id, CancellationToken ct = default);
        Task<(byte[] bytes, string mime)?> GetImageAsync(int id, CancellationToken ct = default);
        Task SoftDeleteAsync(int id, int actedByUserId, string? reason = null, CancellationToken ct = default);
        Task<string> StartHardDeleteAsync(int id, int actedByUserId, CancellationToken ct = default);
        Task UpdateImageAsync(int evidenceId, EvidenceUpdateImageDto dto, CancellationToken ct = default);
        Task UpdateTextAsync(int id, EvidenceUpdateTextDto dto, CancellationToken ct = default);
    }
}