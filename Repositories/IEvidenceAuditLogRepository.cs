// Repositories/Implementations/EvidenceAuditLogRepository.cs
using BERihalCodestackerChallenge2025.Model;

namespace BERihalCodestackerChallenge2025.Repositories
{
    public interface IEvidenceAuditLogRepository // Evidence audit log repository interface extending generic repository for evidence audit log-specific operations
    {
        Task<IEnumerable<EvidenceAuditLog>> GetAdminLogsAsync(DateTime? fromUtc = null, DateTime? toUtc = null, CancellationToken ct = default); // Retrieve audit logs with optional date filtering
    }
}