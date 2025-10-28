using BERihalCodestackerChallenge2025.Data;
using BERihalCodestackerChallenge2025.Model;
using Microsoft.EntityFrameworkCore;

namespace BERihalCodestackerChallenge2025.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository // User repository implementation

    {
        private readonly AppDbContext _context; // Database context
        public UserRepository(AppDbContext context) : base(context) => _context = context; // Constructor accepting the database context

        public async Task<User?> GetByUsernameOrEmailAsync(string usernameOrEmail) // Retrieve a user by username or email
        {
            return await _context.Users 
                .FirstOrDefaultAsync(u => u.Username == usernameOrEmail || u.Email == usernameOrEmail); // Use LINQ to find the user
        }

        public async Task<bool> ExistsByEmailAsync(string email) // Check if a user exists by email
        {
            return await _context.Users.AnyAsync(u => u.Email == email); // Use LINQ to check for existence
        }
    }
}
