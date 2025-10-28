using BERihalCodestackerChallenge2025.Model;

namespace BERihalCodestackerChallenge2025.Repositories
{
    public interface ICaseRepository
    {
        Task<Case?> GetDetailsAsync(int id);
        Task<IEnumerable<Case>> SearchAsync(string? query);
    }
}