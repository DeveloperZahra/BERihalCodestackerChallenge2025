using BERihalCodestackerChallenge2025.Model;
using BERihalCodestackerChallenge2025.Repositories;

namespace BERihalCodestackerChallenge2025.Services
{
    public class AuditLogService : IAuditLogService // Service for managing audit log operations
    {
        private readonly IUnitOfWork _uow;  // Unit of Work for accessing repositories
        public AuditLogService(IUnitOfWork uow) => _uow = uow; // Constructor accepting the Unit of Work

        public Task<IEnumerable<EvidenceAuditLog>> GetEvidenceAdminLogsAsync(DateTime? fromUtc = null, DateTime? toUtc = null, CancellationToken ct = default) // Retrieve evidence audit logs with optional date filtering
            => _uow.EvidenceAudit.GetAdminLogsAsync(fromUtc, toUtc, ct); // Delegate the call to the EvidenceAudit repository
    }
}
