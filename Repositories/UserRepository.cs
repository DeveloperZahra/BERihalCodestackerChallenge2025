// Repositories/Implementations/UserRepository.cs
using BERihalCodestackerChallenge2025.Data;
using BERihalCodestackerChallenge2025.Model;
using Microsoft.EntityFrameworkCore;

namespace BERihalCodestackerChallenge2025.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository // User repository implementation
    {
        public UserRepository(AppDbContext db) : base(db) { } // Constructor accepting the database context

        public Task<User?> GetByUsernameOrEmailAsync(string login, CancellationToken ct = default) // Retrieve a user by username or email
            => _db.Users.AsNoTracking()
               .FirstOrDefaultAsync(u => u.Username == login || u.Email == login, ct); // Find the user by username or email

        public Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default) // Check if a user exists by email
            => _db.Users.AnyAsync(u => u.Email == email, ct); // Check for existence of the email
    }
}

