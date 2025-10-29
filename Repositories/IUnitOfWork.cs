// Repositories/Implementations/UnitOfWork.cs

namespace BERihalCodestackerChallenge2025.Repositories
{
    public interface IUnitOfWork // Unit of Work interface encapsulating all repositories and transaction management
    {
        ICaseAssigneeRepository CaseAssignees { get; } // Case assignee repository
        ICaseParticipantRepository CaseParticipants { get; } // Case participant repository
        ICaseRepository Cases { get; } // Case repository
        IEvidenceRepository Evidence { get; } // Evidence repository
        IEvidenceAuditLogRepository EvidenceAudit { get; } // Evidence audit log repository
        IParticipantRepository Participants { get; } // Participant repository
        IReportRepository Reports { get; } // Report repository
        IUserRepository Users { get; } // User repository

        Task<IDisposable> BeginTransactionAsync(CancellationToken ct = default); // Begin a new database transaction
        Task CommitTransactionAsync(CancellationToken ct = default); // Commit the current database transaction
        ValueTask DisposeAsync(); // Dispose resources asynchronously
        Task RollbackTransactionAsync(CancellationToken ct = default); // Rollback the current database transaction
        Task<int> SaveChangesAsync(CancellationToken ct = default); // Save changes to the database
    }
}