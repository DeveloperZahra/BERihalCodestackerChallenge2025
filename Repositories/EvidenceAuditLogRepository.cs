// Repositories/Implementations/EvidenceAuditLogRepository.cs
using BERihalCodestackerChallenge2025.Data;
using BERihalCodestackerChallenge2025.Model;
using Microsoft.EntityFrameworkCore;

namespace BERihalCodestackerChallenge2025.Repositories
{
    public class EvidenceAuditLogRepository : GenericRepository<EvidenceAuditLog>, IEvidenceAuditLogRepository // Implements repository for managing evidence audit logs
    {
        public EvidenceAuditLogRepository(AppDbContext db) : base(db) { } // Constructor accepting the database context

        public async Task<IEnumerable<EvidenceAuditLog>> GetAdminLogsAsync(DateTime? fromUtc = null, DateTime? toUtc = null, CancellationToken ct = default) // Retrieve audit logs with optional date filtering
        {
            var q = _db.EvidenceAuditLogs.AsNoTracking() // Base query for audit logs
                    .Include(a => a.ActedByUser) // Include the user who performed the action
                    .Include(a => a.Evidence) // Include the related evidence
                    .AsQueryable(); // Make the queryable for further filtering

            if (fromUtc is not null) q = q.Where(a => a.ActedAt >= fromUtc); // Filter logs from the specified start date
            if (toUtc is not null) q = q.Where(a => a.ActedAt <= toUtc); // Filter logs up to the specified end date

            return await q.OrderByDescending(a => a.ActedAt).ToListAsync(ct); // Order results by action date descending and convert to a list
        }
    }
}