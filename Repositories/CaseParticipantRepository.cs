// Repositories/Implementations/CaseParticipantRepository.cs
using BERihalCodestackerChallenge2025.Data;
using BERihalCodestackerChallenge2025.Model;
using Microsoft.EntityFrameworkCore;

namespace BERihalCodestackerChallenge2025.Repositories
{
    public class CaseParticipantRepository : GenericRepository<CaseParticipant>, ICaseParticipantRepository // Implements repository for managing case participants
    {
        public CaseParticipantRepository(AppDbContext db) : base(db) { } // Constructor accepting the database context

        public async Task<IEnumerable<CaseParticipant>> GetByCaseAndRoleAsync(int caseId, ParticipantRole role, CancellationToken ct = default) // Retrieve participants for a specific case and role
            => await _db.CaseParticipants.AsNoTracking() // Use AsNoTracking for read-only operations
                .Include(cp => cp.Participant) // Include the related participant information
                .Include(cp => cp.AddedByUser) // Include the user who added the participant
                .Where(cp => cp.CaseId == caseId && cp.Role == role) // Filter by case ID and participant role
                .ToListAsync(ct);
    }
}
