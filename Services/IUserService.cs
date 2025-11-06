// Services/UserService.cs
using BERihalCodestackerChallenge2025.DTOs;
using BERihalCodestackerChallenge2025.Model;

namespace BERihalCodestackerChallenge2025.Services
{
    public interface IUserService
    {
        Task<UserReadDto> CreateAsync(UserCreateUpdateDto dto, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
        Task<bool> ExistsByUsernameOrEmailAsync(string username, string email, CancellationToken ct = default);
        Task<IEnumerable<UserReadDto>> GetAllAsync(CancellationToken ct = default);
        Task<UserReadDto?> GetByIdAsync(int id, CancellationToken ct = default);
        Task UpdateAsync(int id, UserCreateUpdateDto dto, CancellationToken ct = default);
        Task<User?> ValidateCredentialsAsync(string usernameOrEmail, string password);

    }
}