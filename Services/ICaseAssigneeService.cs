using BERihalCodestackerChallenge2025.DTOs;

namespace BERihalCodestackerChallenge2025.Services
{
    public interface ICaseAssigneeService
    {
        Task<CaseAssigneeReadDto> AssignAsync(CaseAssigneeCreateDto dto, CancellationToken ct = default);
        Task<IReadOnlyList<CaseAssigneeReadDto>> GetByCaseAsync(int caseId, CancellationToken ct = default);
        Task<IReadOnlyList<CaseAssigneeMyAssignmentDto>> GetMyAssignmentsAsync(int userId, CancellationToken ct = default);
        Task<bool> RemoveAsync(int assigneeId, CancellationToken ct = default);
        Task<CaseAssigneeReadDto?> UpdateProgressAsync(int assigneeId, CaseProgressUpdateDto dto, CancellationToken ct = default);
    }
}