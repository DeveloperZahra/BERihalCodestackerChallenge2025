// Services/UserService.cs
using Microsoft.EntityFrameworkCore;
using BERihalCodestackerChallenge2025.Data;
using BERihalCodestackerChallenge2025.DTOs;
using BERihalCodestackerChallenge2025.Model;
using BERihalCodestackerChallenge2025.Repositories;

namespace BERihalCodestackerChallenge2025.Services
{
    // Service for managing user-related operations
    public class UserService : IUserService
    {
        private readonly AppDbContext _db;
        //private readonly IUnitOfWork _uow;
        private readonly IGenericRepository<User> _users;

        public UserService(AppDbContext db, IGenericRepository<User> usersRepo)
        {
            _db = db;
            _users = usersRepo;
        }

        // ============================================================
        // Create new user
        // ============================================================
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
            await _db.SaveChangesAsync(ct);

            return new UserReadDto
            {
                Id = user.UserId,
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
                Username = u.Username,
                Email = u.Email,
                Role = u.Role.ToString(),
                ClearanceLevel = u.ClearanceLevel.ToString(),
                CreatedAt = u.CreatedAt
            };
        }

        // ============================================================
        // Get all users
        // ============================================================
        public async Task<IEnumerable<UserReadDto>> GetAllAsync(CancellationToken ct = default)
        {
            var allUsers = await _users.GetAllAsync(ct);
            return allUsers.Select(u => new UserReadDto
            {
                Id = u.UserId,
                Username = u.Username,
                Email = u.Email,
                Role = u.Role.ToString(),
                ClearanceLevel = u.ClearanceLevel.ToString(),
                CreatedAt = u.CreatedAt
            });
        }

        // ============================================================
        // Check if username or email already exists
        // ============================================================
        public async Task<bool> ExistsByUsernameOrEmailAsync(string username, string email, CancellationToken ct = default)
        {
            var users = await _users.GetAllAsync(ct);
            var exists = users.Any(u => u.Username == username || u.Email == email);
            return exists;
        }

        // ============================================================
        // Update existing user
        // ============================================================
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
            await _db.SaveChangesAsync(ct);
        }
        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var user = await _users.GetByIdAsync(id, ct)
                       ?? throw new KeyNotFoundException("User not found.");

            _users.Delete(user);
            await _db.SaveChangesAsync(ct);
        }
        //----------------------
        // Validate Credentials
        //----------------------
        public async Task<User?> ValidateCredentialsAsync(string usernameOrEmail, string password)
        {
            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.Email == usernameOrEmail || u.Username == usernameOrEmail);

            if (user == null)
                return null;

            
            bool validPassword = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);

            return validPassword ? user : null;
        }


    }
}