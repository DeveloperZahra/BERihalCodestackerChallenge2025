using BERihalCodestackerChallenge2025.DTOs;

namespace BERihalCodestackerChallenge2025.Services
{
    public interface ICaseService
    {
        Task<(int caseId, string caseNumber)> CreateAsync(int createdByUserId, CaseCreateDto dto, CancellationToken ct = default);
    }
}