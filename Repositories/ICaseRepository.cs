using BERihalCodestackerChallenge2025.Model;

namespace BERihalCodestackerChallenge2025.Repositories // Case repository interface for case-specific operations
{
    public interface ICaseRepository // Case repository interface for case-specific operations
    {
        Task<Case?> GetDetailsAsync(int id);
        Task<IEnumerable<Case>> SearchAsync(string? query);
    }
}