using BERihalCodestackerChallenge2025.DTOs;
using BERihalCodestackerChallenge2025.Model;
using BERihalCodestackerChallenge2025.Repositories;

namespace BERihalCodestackerChallenge2025.Services
{
    public class ParticipantService : IParticipantService
    {
        private readonly IUnitOfWork _uow;
        private readonly IGenericRepository<Participant> _participants;

        public ParticipantService(IUnitOfWork uow, IGenericRepository<Participant> participantsRepo)
        {
            _uow = uow;
            _participants = participantsRepo;
        }

        // ================================================================
        // CREATE
        // ================================================================
        public async Task<ParticipantReadDto> CreateAsync(ParticipantCreateUpdateDto dto, CancellationToken ct = default)
        {
            var participant = new Participant
            {
                Name = dto.Name,
                Email = dto.Email,
                Phone = dto.Phone,
                Role = dto.Role,
                CreatedAt = DateTime.UtcNow
            };

            await _participants.AddAsync(participant, ct);
            await _uow.SaveChangesAsync(ct);

            return new ParticipantReadDto
            {
                Id = participant.Id,
                Name = participant.Name,
                Email = participant.Email,
                Phone = participant.Phone,
                Role = participant.Role,
                CreatedAt = participant.CreatedAt
            };
        }

        // ================================================================
        // READ ALL
        // ================================================================
        public async Task<IEnumerable<ParticipantReadDto>> GetAllAsync(CancellationToken ct = default)
        {
            var list = await _participants.GetAllAsync(ct);
            return list.Select(p => new ParticipantReadDto
            {
                Id = p.Id,
                Name = p.Name,
                Email = p.Email,
                Phone = p.Phone,
                Role = p.Role,
                CreatedAt = p.CreatedAt
            });
        }

        // ================================================================
        // READ BY ID
        // ================================================================
        public async Task<ParticipantReadDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var participant = await _participants.GetByIdAsync(id, ct);
            if (participant is null) return null;

            return new ParticipantReadDto
            {
                Id = participant.Id,
                Name = participant.Name,
                Email = participant.Email,
                Phone = participant.Phone,
                Role = participant.Role,
                CreatedAt = participant.CreatedAt
            };
        }

        // ================================================================
        // UPDATE
        // ================================================================
        public async Task UpdateAsync(int id, ParticipantCreateUpdateDto dto, CancellationToken ct = default)
        {
            var participant = await _participants.GetByIdAsync(id, ct)
                               ?? throw new KeyNotFoundException("Participant not found.");

            participant.Name = dto.Name;
            participant.Email = dto.Email;
            participant.Phone = dto.Phone;
            participant.Role = dto.Role;
            participant.UpdatedAt = DateTime.UtcNow;

            _participants.Update(participant);
            await _uow.SaveChangesAsync(ct);
        }

        // ================================================================
        // DELETE
        // ================================================================
        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var participant = await _participants.GetByIdAsync(id, ct)
                               ?? throw new KeyNotFoundException("Participant not found.");

            _participants.Delete(participant);
            await _uow.SaveChangesAsync(ct);
        }
    }
}
