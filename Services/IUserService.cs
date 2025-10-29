using BERihalCodestackerChallenge2025.DTOs;

namespace BERihalCodestackerChallenge2025.Services
{
    public interface IUserService // User service interface defining user-related operations
    {
        Task<UserReadDto> CreateAsync(UserCreateUpdateDto dto, CancellationToken ct = default); // Create a new user and return the created user data
        Task DeleteAsync(int id, CancellationToken ct = default); // Delete a user by their ID
        Task<UserReadDto?> GetByIdAsync(int id, CancellationToken ct = default); // Retrieve a user by their ID
        Task UpdateAsync(int id, UserCreateUpdateDto dto, CancellationToken ct = default); // Update an existing user's information
    }
}