using BERihalCodestackerChallenge2025.Model;

namespace BERihalCodestackerChallenge2025.Repositories // Case repository interface for case-specific operations
{
    public interface ICaseRepository // Case repository interface for case-specific operations
    {
        Task<Case?> GetDetailsAsync(int id); // Retrieve detailed information about a Case by its ID
        Task<IEnumerable<Case>> SearchAsync(string? query); // Search for cases based on a query string
    }
}