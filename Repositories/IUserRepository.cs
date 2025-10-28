using BERihalCodestackerChallenge2025.Model;

namespace BERihalCodestackerChallenge2025.Repositories
{
    public interface IUserRepository // User repository interface for user-specific operations
    {
        Task<bool> ExistsByEmailAsync(string email); 
        Task<User?> GetByUsernameOrEmailAsync(string usernameOrEmail); 
    }
}