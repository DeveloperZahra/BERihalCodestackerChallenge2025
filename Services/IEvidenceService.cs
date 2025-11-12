using BERihalCodestackerChallenge2025.DTOs;

namespace BERihalCodestackerChallenge2025.Services
{
    public interface IEvidenceService
    {
        Task<string> ConfirmHardDeleteAsync(int id, int actedByUserId, string yesNo);
        Task<int> CreateTextAsync(int caseId, int addedByUserId, EvidenceCreateDto dto, CancellationToken ct = default);
        Task<bool> FinalizeHardDeleteAsync(int id, int actedByUserId, string command, CancellationToken ct = default);
        Task<EvidenceReadDto?> GetAsync(int id, CancellationToken ct = default);
        Task<(byte[] Bytes, string Mime)> GetImageAsync(int id, CancellationToken ct = default);
        Task SoftDeleteAsync(int id, int actedByUserId, string? reason = null, CancellationToken ct = default);
        Task<string> StartHardDeleteAsync(int id, int actedByUserId);
        Task UpdateImageAsync(int evidenceId, EvidenceUpdateImageDto dto, CancellationToken ct = default);
        Task UpdateTextAsync(int id, EvidenceUpdateTextDto dto, CancellationToken ct = default);
    }
}