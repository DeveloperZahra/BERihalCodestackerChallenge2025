// Repositories/Implementations/CaseAssigneeRepository.cs
using BERihalCodestackerChallenge2025.Data;
using BERihalCodestackerChallenge2025.Model;
using Microsoft.EntityFrameworkCore;

namespace BERihalCodestackerChallenge2025.Repositories
{
    public class CaseAssigneeRepository : GenericRepository<CaseAssignee>, ICaseAssigneeRepository // Implement the case assignee repository interface
    {
        public CaseAssigneeRepository(AppDbContext db) : base(db) { } // Constructor accepting the database context

        public Task<bool> ExistsAsync(int caseId, int userId, CancellationToken ct = default) // Check if a specific user is assigned to a specific case
            => _db.CaseAssignees.AnyAsync(a => a.CaseId == caseId && a.UserId == userId, ct); // Check for existence of the assignment

        public async Task<IEnumerable<CaseAssignee>> GetByCaseAsync(int caseId, CancellationToken ct = default) // Retrieve all assignees for a specific case
            => await _db.CaseAssignees.AsNoTracking() 
                   .Include(a => a.User)
                   .Where(a => a.CaseId == caseId)
                   .ToListAsync(ct); // Convert the results to a list

        public async Task<IEnumerable<CaseAssignee>> GetMyAssignmentsAsync(int userId, CancellationToken ct = default) // Retrieve all case assignments for a specific user
            => await _db.CaseAssignees.AsNoTracking() 
                   .Include(a => a.Case) // Include the related case information
                   .Where(a => a.UserId == userId) // Filter by user ID
                   .ToListAsync(ct); // Convert the results to a list
    }
}