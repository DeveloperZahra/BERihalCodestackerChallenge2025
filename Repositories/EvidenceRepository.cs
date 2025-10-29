// Repositories/Implementations/EvidenceRepository.cs
using BERihalCodestackerChallenge2025.Data;
using BERihalCodestackerChallenge2025.Model;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace BERihalCodestackerChallenge2025.Repositories
{
    public class EvidenceRepository : GenericRepository<Evidence>, IEvidenceRepository // Implements evidence-specific data operations
    {
        public EvidenceRepository(AppDbContext db) : base(db) { } // Constructor accepting the database context
        // add new evidence 

        
        public Task<Evidence?> GetWithUserAsync(int id, CancellationToken ct = default) // Retrieve evidence by ID including the user who added it
            => _db.Evidences.Include(e => e.AddedByUser).FirstOrDefaultAsync(e => e.Id == id, ct); // Find the evidence by ID

        public async Task<IEnumerable<Evidence>> GetByCaseAsync(int caseId, bool includeSoftDeleted = false, CancellationToken ct = default) // Retrieve all evidences for a specific case, with an option to include soft-deleted records
        {
            var q = _db.Evidences.AsNoTracking().Where(e => e.CaseId == caseId); // Base query filtering by case ID
            if (!includeSoftDeleted) q = q.Where(e => !e.IsSoftDeleted); // Exclude soft-deleted records if specified
            return await q.ToListAsync(ct); // Convert the results to a list
        }

        public async Task SoftDeleteAsync(Evidence entity, int actedByUserId, string? details = null, CancellationToken ct = default) // Soft delete an evidence record and log the action
        {
            entity.IsSoftDeleted = true; // Mark the evidence as soft-deleted
            entity.UpdatedAt = DateTime.UtcNow; // Update the timestamp
            _db.EvidenceAuditLogs.Add(new EvidenceAuditLog // Log the soft delete action
            {
                EvidenceId = entity.Id, // ID of the evidence being soft-deleted
                Action = "soft_delete", // Action type
                ActedByUserId = actedByUserId, // ID of the user performing the action
                ActedAt = DateTime.UtcNow, // Timestamp of the action
                Details = details // Additional details about the action
            });
            await Task.CompletedTask; // Complete the asynchronous operation
        }

        public async Task HardDeleteAsync(Evidence entity, int actedByUserId, string? details = null, CancellationToken ct = default) // Hard delete an evidence record and log the action
        {
            _db.EvidenceAuditLogs.Add(new EvidenceAuditLog // Log the hard delete action
            {
                EvidenceId = entity.Id, // ID of the evidence being hard-deleted
                Action = "hard_delete", // Action type
                ActedByUserId = actedByUserId, // ID of the user performing the action
                ActedAt = DateTime.UtcNow, // Timestamp of the action
                Details = details // Additional details about the action
            });
            _db.Evidences.Remove(entity); // Remove the evidence from the database
            await Task.CompletedTask; // Complete the asynchronous operation
        }
    }
}

