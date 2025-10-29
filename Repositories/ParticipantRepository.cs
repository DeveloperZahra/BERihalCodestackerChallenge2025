// Repositories/Implementations/ParticipantRepository.cs
using BERihalCodestackerChallenge2025.Data;
using BERihalCodestackerChallenge2025.Model;
using Microsoft.EntityFrameworkCore;

namespace BERihalCodestackerChallenge2025.Repositories
{
    public class ParticipantRepository : GenericRepository<Participant>, IParticipantRepository // Implementing participant-specific data operations
    {
        public ParticipantRepository(AppDbContext db) : base(db) { } // Constructor accepting the database context

        public Task<Participant?> GetByNameAsync(string fullName, CancellationToken ct = default) // Retrieve a participant by their full name
            => _db.Participants.AsNoTracking().FirstOrDefaultAsync(p => p.FullName == fullName, ct); // Find the participant by full name
    }
}

