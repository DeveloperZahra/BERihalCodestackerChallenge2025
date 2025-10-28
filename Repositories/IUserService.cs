using BERihalCodestackerChallenge2025.DTOs;

namespace BERihalCodestackerChallenge2025.Services
{
    public interface IUserService
    {
        Task<UserReadDto> CreateAsync(UserCreateUpdateDto dto);
        Task DeleteAsync(int id);
        Task<UserReadDto?> GetByIdAsync(int id);
        Task UpdateAsync(int id, UserCreateUpdateDto dto);
    }
}