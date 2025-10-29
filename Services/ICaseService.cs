using BERihalCodestackerChallenge2025.DTOs;
using BERihalCodestackerChallenge2025.Model;

namespace BERihalCodestackerChallenge2025.Services
{
    public interface ICaseService
    {
        Task AssignOfficerAsync(int caseId, int officerUserId, CancellationToken ct = default);
        Task<(int caseId, string caseNumber)> CreateAsync(int createdByUserId, CaseCreateDto dto, CancellationToken ct = default);
        Task<object?> GetDetailsAsync(int id, CancellationToken ct = default);
        Task<IEnumerable<CaseListItemDto>> ListAsync(string? q, CancellationToken ct = default);
        Task SetStatusAsync(int caseId, CaseStatus status, CancellationToken ct = default);
        Task UpdateAsync(int caseId, CaseUpdateDto dto, CancellationToken ct = default);
    }
}