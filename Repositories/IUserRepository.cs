// Repositories/Implementations/UserRepository.cs
using BERihalCodestackerChallenge2025.Model;

namespace BERihalCodestackerChallenge2025.Repositories
{
    public interface IUserRepository // User repository interface extending generic repository for user-specific operations
    {
        Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default); // Check if a user exists by email
        Task<User?> GetByUsernameOrEmailAsync(string login, CancellationToken ct = default); // Retrieve a user by username or email
    }
}