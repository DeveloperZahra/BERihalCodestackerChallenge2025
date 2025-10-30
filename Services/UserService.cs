// Services/UserService.cs
using BERihalCodestackerChallenge2025.DTOs;
using BERihalCodestackerChallenge2025.Model;
using BERihalCodestackerChallenge2025.Repositories;

namespace BERihalCodestackerChallenge2025.Services
{
    // Service for managing user-related operations
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _uow;                  
        private readonly IGenericRepository<User> _users;   

        public UserService(IUnitOfWork uow, IGenericRepository<User> usersRepo)
        {
            _uow = uow;
            _users = usersRepo;
        }

        public async Task<UserReadDto> CreateAsync(UserCreateUpdateDto dto, CancellationToken ct = default)
        {
            if (!Enum.TryParse<Role>(dto.Role, true, out var role))
                throw new ArgumentException("Invalid role.");
            if (!Enum.TryParse<Clearance>(dto.ClearanceLevel, true, out var level))
                throw new ArgumentException("Invalid clearance level.");

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = role,
                ClearanceLevel = level,
                CreatedAt = DateTime.UtcNow
            };

            await _users.AddAsync(user, ct);
            await _uow.SaveChangesAsync(ct);

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

        public async Task<UserReadDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var u = await _users.GetByIdAsync(id, ct);
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

        public async Task UpdateAsync(int id, UserCreateUpdateDto dto, CancellationToken ct = default)
        {
            var user = await _users.GetByIdAsync(id, ct)
                       ?? throw new KeyNotFoundException("User not found.");

            if (!Enum.TryParse<Role>(dto.Role, true, out var role))
                throw new ArgumentException("Invalid role.");
            if (!Enum.TryParse<Clearance>(dto.ClearanceLevel, true, out var level))
                throw new ArgumentException("Invalid clearance level.");

            user.Username = dto.Username;
            user.Email = dto.Email;
            user.Role = role;
            user.ClearanceLevel = level;

            if (!string.IsNullOrWhiteSpace(dto.Password))
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            _users.Update(user);
            await _uow.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var user = await _users.GetByIdAsync(id, ct)
                       ?? throw new KeyNotFoundException("User not found.");

            _users.Delete(user);
            await _uow.SaveChangesAsync(ct);
        }
    }
}


