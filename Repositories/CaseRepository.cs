// Repositories/Implementations/CaseRepository.cs
using BERihalCodestackerChallenge2025.Data;
using BERihalCodestackerChallenge2025.Model;
using Microsoft.EntityFrameworkCore;

namespace BERihalCodestackerChallenge2025.Repositories
{
    public class CaseRepository : GenericRepository<Case>, ICaseRepository // Implementing case-specific data operations
    {
        public CaseRepository(AppDbContext db) : base(db) { } // Constructor accepting the database context

        public async Task<IEnumerable<Case>> SearchAsync(string? q, CancellationToken ct = default) // Search cases by query string in name or description
        {
            var query = _db.Cases.AsNoTracking().Include(c => c.CreatedByUser).AsQueryable(); // Base query including the user who created the case
            if (!string.IsNullOrWhiteSpace(q)) // If a search query is provided
                query = query.Where(c => EF.Functions.ILike(c.Name, $"%{q}%") || EF.Functions.ILike(c.Description, $"%{q}%")); // Case-insensitive search in name and description
            return await query.OrderByDescending(c => c.CreatedAt).ToListAsync(ct); // Order results by creation date descending and convert to a list
        }

        public Task<Case?> GetDetailsAsync(int id, CancellationToken ct = default) // Retrieve detailed information about a case by its ID
            => _db.Cases
                  .Include(c => c.CreatedByUser) // Include the user who created the case
                  .Include(c => c.Assignees).ThenInclude(a => a.User) // Include assigned users
                  .Include(c => c.Evidences) // Include evidences linked to the case
                  .Include(c => c.Participants).ThenInclude(p => p.Participant) // Include participants involved in the case
                  .Include(c => c.LinkedReports).ThenInclude(l => l.Report).ThenInclude(r => r.ReportedByUser) // Include linked reports and their reporting users
                  .FirstOrDefaultAsync(c => c.Id == id, ct); // Find the case by ID

        public Task<bool> ExistsByNumberAsync(string caseNumber, CancellationToken ct = default)// Check if a case exists by its case number
            => _db.Cases.AnyAsync(c => c.CaseNumber == caseNumber, ct); // Check for existence of the case number

        public async Task LinkReportsAsync(int caseId, IEnumerable<int> reportIds, CancellationToken ct = default) // Link multiple reports to a case
        {
            var existing = await _db.CaseReports.Where(x => x.CaseId == caseId).Select(x => x.ReportId).ToListAsync(ct); // Get existing linked report IDs for the case
            var toAdd = reportIds.Distinct().Except(existing).Select(rid => new CaseReport // Create new CaseReport entries for reports not already linked
            {
                CaseId = caseId, // Set the case ID
                ReportId = rid,
                LinkedAt = DateTime.UtcNow // Set the current timestamp for when the report is linked
            });
            await _db.CaseReports.AddRangeAsync(toAdd, ct); // Add the new CaseReport entries to the database
        }
    }
}
