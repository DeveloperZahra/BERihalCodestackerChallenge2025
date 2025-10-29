// Repositories/Implementations/CaseAssigneeRepository.cs
using BERihalCodestackerChallenge2025.Model;

namespace BERihalCodestackerChallenge2025.Repositories
{
    public interface ICaseAssigneeRepository // Case assignee repository interface extending generic repository for case assignee-specific operations
    {
        Task<bool> ExistsAsync(int caseId, int userId, CancellationToken ct = default); // Check if a specific user is assigned to a specific case
        Task<IEnumerable<CaseAssignee>> GetByCaseAsync(int caseId, CancellationToken ct = default); // Retrieve all assignees for a specific case
        Task<IEnumerable<CaseAssignee>> GetMyAssignmentsAsync(int userId, CancellationToken ct = default); // Retrieve all case assignments for a specific user
    }
}