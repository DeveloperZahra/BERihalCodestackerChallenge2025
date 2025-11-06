using BERihalCodestackerChallenge2025.Model;

namespace BERihalCodestackerChallenge2025.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}