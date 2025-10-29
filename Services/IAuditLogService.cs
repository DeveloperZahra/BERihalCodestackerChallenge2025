using BERihalCodestackerChallenge2025.Model;

namespace BERihalCodestackerChallenge2025.Services
{
    public interface IAuditLogService // Audit log service interface for retrieving evidence audit logs
    {
        Task<IEnumerable<EvidenceAuditLog>> GetEvidenceAdminLogsAsync(DateTime? fromUtc = null, DateTime? toUtc = null, CancellationToken ct = default); // Retrieve evidence audit logs with optional date filtering
    }
}