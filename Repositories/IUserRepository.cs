using BERihalCodestackerChallenge2025.Model;

namespace BERihalCodestackerChallenge2025.Repositories
{
    public interface IUserRepository // User repository interface for user-specific operations
    {
        Task<bool> ExistsByEmailAsync(string email); // Check if a user exists by email
        Task<User?> GetByUsernameOrEmailAsync(string usernameOrEmail); // Retrieve a user by username or email
    }
}