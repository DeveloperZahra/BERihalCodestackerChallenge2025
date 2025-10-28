using BERihalCodestackerChallenge2025.DTOs;
using BERihalCodestackerChallenge2025.Model;
using BERihalCodestackerChallenge2025.Repositories;

namespace BERihalCodestackerChallenge2025.Services
{
    public class UserService : IUserService // User service implementation for managing users

    {
        private readonly IUserRepository _users;
        public UserService(IUserRepository users) => _users = users;

        public async Task<UserReadDto> CreateAsync(UserCreateUpdateDto dto)
        {
            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = Enum.Parse<Role>(dto.Role, true),
                ClearanceLevel = Enum.Parse<Clearance>(dto.ClearanceLevel, true),
                CreatedAt = DateTime.UtcNow
            };
            await _users.AddAsync(user);
            await _users.SaveAsync();

            return new UserReadDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role.ToString(),
                ClearanceLevel = user.ClearanceLevel.ToString(),
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<UserReadDto?> GetByIdAsync(int id)
        {
            var u = await _users.GetByIdAsync(id);
            if (u is null) return null;
            return new UserReadDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                Role = u.Role.ToString(),
                ClearanceLevel = u.ClearanceLevel.ToString(),
                CreatedAt = u.CreatedAt
            };
        }

        public async Task UpdateAsync(int id, UserCreateUpdateDto dto)
        {
            var user = await _users.GetByIdAsync(id);
            if (user is null) throw new KeyNotFoundException("User not found.");

            user.Username = dto.Username;
            user.Email = dto.Email;
            user.Role = Enum.Parse<Role>(dto.Role, true);
            user.ClearanceLevel = Enum.Parse<Clearance>(dto.ClearanceLevel, true);
            if (!string.IsNullOrWhiteSpace(dto.Password))
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            _users.Update(user);
            await _users.SaveAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var user = await _users.GetByIdAsync(id);
            if (user is null) throw new KeyNotFoundException("User not found.");
            _users.Delete(user);
            await _users.SaveAsync();
        }
    }
}

