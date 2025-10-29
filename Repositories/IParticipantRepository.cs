// Repositories/Implementations/ParticipantRepository.cs
using BERihalCodestackerChallenge2025.Model;

namespace BERihalCodestackerChallenge2025.Repositories
{
    public interface IParticipantRepository
    {
        Task<Participant?> GetByNameAsync(string fullName, CancellationToken ct = default);
    }
}