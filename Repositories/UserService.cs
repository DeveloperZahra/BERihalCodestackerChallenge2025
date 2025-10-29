using BERihalCodestackerChallenge2025.DTOs;
using BERihalCodestackerChallenge2025.Model;
using BERihalCodestackerChallenge2025.Repositories;

namespace BERihalCodestackerChallenge2025.Services
{
    public class UserService : IUserService // User service implementation for managing users

    {
        private readonly IUserRepository _users; // User repository for data access
        public UserService(IUserRepository users) => _users = users; // Constructor accepting the user repository

        public async Task<UserReadDto> CreateAsync(UserCreateUpdateDto dto) // Create a new user
        {
            var user = new User // Create a new User entity from the DTO
            {
                Username = dto.Username, 
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password), // Hash the password for security
                Role = Enum.Parse<Role>(dto.Role, true), // Parse role from string
                ClearanceLevel = Enum.Parse<Clearance>(dto.ClearanceLevel, true), // Parse clearance level from string
                CreatedAt = DateTime.UtcNow // Set creation timestamp
            };
            await _users.AddAsync(user); // Add the new user to the repository
            await _users.SaveAsync(); // Save changes to the database

            return new UserReadDto // Return a DTO representing the created user
            {
                Id = user.Id,// Assign the generated Id
                Username = user.Username, 
                Email = user.Email, 
                Role = user.Role.ToString(),
                ClearanceLevel = user.ClearanceLevel.ToString(), 
                CreatedAt = user.CreatedAt // Assign the creation timestamp
            };
        }

        public async Task<UserReadDto?> GetByIdAsync(int id) // Retrieve a user by ID
        {
            var u = await _users.GetByIdAsync(id); // Get the user entity from the repository
            if (u is null) return null; // If not found, return null
            return new UserReadDto // Return a DTO representing the user
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                Role = u.Role.ToString(), 
                ClearanceLevel = u.ClearanceLevel.ToString(),
                CreatedAt = u.CreatedAt // Assign the creation timestamp
            };
        }

        public async Task UpdateAsync(int id, UserCreateUpdateDto dto)// Update an existing user
        {
            var user = await _users.GetByIdAsync(id); // Get the user entity from the repository
            if (user is null) throw new KeyNotFoundException("User not found."); // If not found, throw an exception

            user.Username = dto.Username; // Update username
            user.Email = dto.Email;
            user.Role = Enum.Parse<Role>(dto.Role, true); // Update role
            user.ClearanceLevel = Enum.Parse<Clearance>(dto.ClearanceLevel, true); // Update clearance level
            if (!string.IsNullOrWhiteSpace(dto.Password)) // Update password if provided
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password); // Hash the new password

            _users.Update(user); // Update the user in the repository
            await _users.SaveAsync(); // Save changes to the database
        }

        public async Task DeleteAsync(int id) // Delete a user by ID
        {
            var user = await _users.GetByIdAsync(id); // Get the user entity from the repository
            if (user is null) throw new KeyNotFoundException("User not found."); // If not found, throw an exception
            _users.Delete(user); // Delete the user from the repository
            await _users.SaveAsync(); // Save changes to the database
        }
    }
}

