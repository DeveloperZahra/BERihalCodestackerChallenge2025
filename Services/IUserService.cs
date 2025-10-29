using BERihalCodestackerChallenge2025.DTOs;

namespace BERihalCodestackerChallenge2025.Services
{
    public interface IUserService
    {
        Task<UserReadDto> CreateAsync(UserCreateUpdateDto dto, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
        Task<UserReadDto?> GetByIdAsync(int id, CancellationToken ct = default);
        Task UpdateAsync(int id, UserCreateUpdateDto dto, CancellationToken ct = default);
    }
}