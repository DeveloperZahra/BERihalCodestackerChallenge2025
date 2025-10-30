// Repositories/Implementations/CaseRepository.cs
using BERihalCodestackerChallenge2025.Data;
using BERihalCodestackerChallenge2025.Model;
using Microsoft.EntityFrameworkCore;

namespace BERihalCodestackerChallenge2025.Repositories
{
    public class CaseRepository : GenericRepository<Case>, ICaseRepository // Implement the case repository interface
    {
        public CaseRepository(AppDbContext db) : base(db) { } // Constructor accepting the database context

        // Search cases by query string in name or description (SQL Server)
        public async Task<IEnumerable<Case>> SearchAsync(string? q, CancellationToken ct = default) // Search cases by query string
        {
            // Base query including the user who created the case
            var query = _db.Cases
                .AsNoTracking() // Avoid tracking for read-only operations
                .Include(c => c.CreatedByUser) // Include the user who created the case
                .AsQueryable(); // Make the queryable for further filtering

            if (!string.IsNullOrWhiteSpace(q)) // If a search query is provided
            {
                var pattern = $"%{q}%"; // Prepare the search pattern for SQL LIKE


                query = query.Where(c =>
                    EF.Functions.Like(
                        EF.Functions.Collate(c.Name, "SQL_Latin1_General_CP1_CI_AS"), 
                        pattern)
                    ||
                    EF.Functions.Like(
                        EF.Functions.Collate(c.Description, "SQL_Latin1_General_CP1_CI_AS"),
                        pattern)
                );
            }

            // Order results by creation date descending and convert to a list
            return await query.OrderByDescending(c => c.CreatedAt).ToListAsync(ct);
        }

        // Retrieve detailed information about a case by its ID
        public Task<Case?> GetDetailsAsync(int id, CancellationToken ct = default)
            => _db.Cases
                  .Include(c => c.CreatedByUser) // Include the user who created the case
                  .Include(c => c.Assignees).ThenInclude(a => a.User) // Include assigned users
                  .Include(c => c.Evidences) // Include evidences linked to the case
                  .Include(c => c.Participants).ThenInclude(p => p.Participant) // Include participants involved in the case
                  .Include(c => c.LinkedReports).ThenInclude(l => l.Report).ThenInclude(r => r.ReportedByUser) // Include linked reports and their reporting users
                  .FirstOrDefaultAsync(c => c.Id == id, ct); // Find the case by ID

        // Check if a case exists by its case number
        public Task<bool> ExistsByNumberAsync(string caseNumber, CancellationToken ct = default)
            => _db.Cases.AnyAsync(c => c.CaseNumber == caseNumber, ct);

        // Link multiple reports to a case
        public async Task LinkReportsAsync(int caseId, IEnumerable<int> reportIds, CancellationToken ct = default) // Link reports to a case
        {
            var existing = await _db.CaseReports
                                    .Where(x => x.CaseId == caseId) // Get existing linked report IDs for the case
                                    .Select(x => x.ReportId) // Select only the ReportId
                                    .ToListAsync(ct); // Convert to a list

            var toAdd = reportIds.Distinct() // Ensure unique report IDs
                                 .Except(existing) // Exclude already linked report IDs
                                 .Select(rid => new CaseReport // Create new CaseReport entities for linking
                                 {
                                     CaseId = caseId,
                                     ReportId = rid,
                                     LinkedAt = DateTime.UtcNow
                                 });

            await _db.CaseReports.AddRangeAsync(toAdd, ct); // Add the new links to the database
        }
    }
}
