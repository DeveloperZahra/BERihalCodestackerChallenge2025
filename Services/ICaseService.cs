using BERihalCodestackerChallenge2025.DTOs;

namespace BERihalCodestackerChallenge2025.Services
{
    public interface ICaseService
    {
        Task<(int caseId, string caseNumber)> CreateAsync(int createdByUserId, CaseCreateDto dto, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct);
        Task<IEnumerable<CaseReadDto>> GetAllAsync(CancellationToken ct);
        Task<CaseListItemDto?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<CaseListItemDto?> UpdateAsync(int id, CaseUpdateDto dto, CancellationToken ct = default);
    }
}