// Repositories/Implementations/UnitOfWork.cs

namespace BERihalCodestackerChallenge2025.Repositories
{
    public interface IUnitOfWork
    {
        ICaseAssigneeRepository CaseAssignees { get; }
        ICaseParticipantRepository CaseParticipants { get; }
        ICaseRepository Cases { get; }
        IEvidenceRepository Evidence { get; }
        IEvidenceAuditLogRepository EvidenceAudit { get; }
        IParticipantRepository Participants { get; }
        IReportRepository Reports { get; }
        IUserRepository Users { get; }

        Task<IDisposable> BeginTransactionAsync(CancellationToken ct = default);
        Task CommitTransactionAsync(CancellationToken ct = default);
        ValueTask DisposeAsync();
        Task RollbackTransactionAsync(CancellationToken ct = default);
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}