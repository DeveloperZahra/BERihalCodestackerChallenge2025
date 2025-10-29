// Repositories/Implementations/UnitOfWork.cs
using BERihalCodestackerChallenge2025.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace BERihalCodestackerChallenge2025.Repositories
{
    public class UnitOfWork : IUnitOfWork // Implements the Unit of Work pattern for managing repositories and transactions
    {
        private readonly AppDbContext _db; // Database context for data access
        private IDbContextTransaction? _txn; // Current database transaction

        public IUserRepository Users { get; } // User repository for managing user data
        public ICaseRepository Cases { get; } // Case repository for managing case data
        public IReportRepository Reports { get; } // Report repository for managing report data
        public ICaseAssigneeRepository CaseAssignees { get; } // Case assignee repository for managing case assignments
        public IParticipantRepository Participants { get; } // Participant repository for managing participant data
        public ICaseParticipantRepository CaseParticipants { get; } // Case participant repository for managing case-participant relationships
        public IEvidenceRepository Evidence { get; } // Evidence repository for managing evidence data
        public IEvidenceAuditLogRepository EvidenceAudit { get; } // Evidence audit log repository for managing audit logs

        public UnitOfWork( // Constructor accepting all required repositories and the database context
            AppDbContext db, 
            IUserRepository users, 
            ICaseRepository cases,
            IReportRepository reports,
            ICaseAssigneeRepository assignees,
            IParticipantRepository participants,
            ICaseParticipantRepository caseParticipants,
            IEvidenceRepository evidence, 
            IEvidenceAuditLogRepository evidenceAudit) // Initialize the UnitOfWork with the provided repositories and database context
        {
            _db = db; // Assign the database context
            Users = users;  // Assign the user repository
            Cases = cases; // Assign the case repository
            Reports = reports; // Assign the report repository
            CaseAssignees = assignees; // Assign the case assignee repository
            Participants = participants; // Assign the participant repository
            CaseParticipants = caseParticipants; // Assign the case participant repository
            Evidence = evidence; // Assign the evidence repository
            EvidenceAudit = evidenceAudit; // Assign the evidence audit log repository
        }

        public async Task<int> SaveChangesAsync(CancellationToken ct = default) // Persist changes to the database
            => await _db.SaveChangesAsync(ct); // Save all changes made in the context to the database

        public async Task<IDisposable> BeginTransactionAsync(CancellationToken ct = default) // Begin a new database transaction
        {
            _txn = await _db.Database.BeginTransactionAsync(ct); // Start a new transaction
            return _txn; // Return the transaction object for disposal
        }

        public async Task CommitTransactionAsync(CancellationToken ct = default) // Commit the current database transaction
        {
            if (_txn != null) await _txn.CommitAsync(ct); // Commit the transaction if it exists
        }

        public async Task RollbackTransactionAsync(CancellationToken ct = default) // Rollback the current database transaction
        {
            if (_txn != null) await _txn.RollbackAsync(ct); // Rollback the transaction if it exists
        }

        public ValueTask DisposeAsync() => _db.DisposeAsync();
    }
}
