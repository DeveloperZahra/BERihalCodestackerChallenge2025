using BERihalCodestackerChallenge2025.Model;

namespace BERihalCodestackerChallenge2025.Repositories
{
    public interface IUserRepository 
    {
        Task<bool> ExistsByEmailAsync(string email); 
        Task<User?> GetByUsernameOrEmailAsync(string usernameOrEmail); 
    }
}