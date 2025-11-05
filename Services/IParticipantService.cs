using BERihalCodestackerChallenge2025.DTOs;

namespace BERihalCodestackerChallenge2025.Services
{
    public interface IParticipantService
    {
        Task<ParticipantReadDto> CreateAsync(ParticipantCreateUpdateDto dto, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
        Task<IEnumerable<ParticipantReadDto>> GetAllAsync(CancellationToken ct = default);
        Task<ParticipantReadDto?> GetByIdAsync(int id, CancellationToken ct = default);
        Task UpdateAsync(int id, ParticipantCreateUpdateDto dto, CancellationToken ct = default);
    }
}