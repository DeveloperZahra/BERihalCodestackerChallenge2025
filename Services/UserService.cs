using BERihalCodestackerChallenge2025.DTOs;
using BERihalCodestackerChallenge2025.Model;
using BERihalCodestackerChallenge2025.Repositories;

namespace BERihalCodestackerChallenge2025.Services
{
    public class UserService : IUserService // Service for managing user-related operations
    {
        private readonly IUnitOfWork _uow; // Unit of Work for accessing repositories
        public UserService(IUnitOfWork uow) => _uow = uow; // Constructor accepting the Unit of Work

        public async Task<UserReadDto> CreateAsync(UserCreateUpdateDto dto, CancellationToken ct = default) // Create a new user
        {
            var user = new User // Create a new User entity from the provided DTO
            {
                Username = dto.Username, 
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password), // Hash the password for secure storage
                Role = Enum.Parse<Role>(dto.Role, true),
                ClearanceLevel = Enum.Parse<Clearance>(dto.ClearanceLevel, true),  // Parse role and clearance level from strings
                CreatedAt = DateTime.UtcNow // Set the creation timestamp
            };
            await _uow.Users.AddAsync(user, ct); // Add the new user to the repository
            await _uow.SaveChangesAsync(ct); // Persist changes to the database

            return new UserReadDto // Return a DTO representing the created user
            {
                Id = user.UserId, 
                Username = user.Username, 
                Email = user.Email,
                Role = user.Role.ToString(),
                ClearanceLevel = user.ClearanceLevel.ToString(),
                CreatedAt = user.CreatedAt
            }; // Return the created user details
        }

        public async Task<UserReadDto?> GetByIdAsync(int id, CancellationToken ct = default) // Retrieve a user by ID
        {
            var u = await _uow.Users.GetByIdAsync(id, ct); // Get the user entity from the repository
            if (u is null) return null; // Return null if the user is not found
            return new UserReadDto // Map the user entity to a DTO
            {
                Id = u.Id, 
                Username = u.Username,
                Email = u.Email,
                Role = u.Role.ToString(),
                ClearanceLevel = u.ClearanceLevel.ToString(),
                CreatedAt = u.CreatedAt
            }; // Return the user details
        }

        public async Task UpdateAsync(int id, UserCreateUpdateDto dto, CancellationToken ct = default) // Update an existing user
        {
            var user = await _uow.Users.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException("User not found."); // Get the user entity or throw if not found
            user.Username = dto.Username; // Update username
            user.Email = dto.Email; // Update email
            user.Role = Enum.Parse<Role>(dto.Role, true);
            user.ClearanceLevel = Enum.Parse<Clearance>(dto.ClearanceLevel, true); // Update role and clearance level
            if (!string.IsNullOrWhiteSpace(dto.Password))
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password); // Update password if provided

            _uow.Users.Update(user); // Mark the user entity as updated
            await _uow.SaveChangesAsync(ct); // Persist changes to the database
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default) // Delete a user by ID
        {
            var user = await _uow.Users.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException("User not found."); // Get the user entity or throw if not found
            _uow.Users.Delete(user); // Mark the user entity for deletion
            await _uow.SaveChangesAsync(ct); // Persist changes to the database
        }
    }
}

