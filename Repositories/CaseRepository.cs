using BERihalCodestackerChallenge2025.Data;
using BERihalCodestackerChallenge2025.Model;
using Microsoft.EntityFrameworkCore;

namespace BERihalCodestackerChallenge2025.Repositories
{
    public class CaseRepository : GenericRepository<Case>, ICaseRepository // Repository for Case entity with specific methods
    {
        private readonly AppDbContext _context; // Database context
        public CaseRepository(AppDbContext context) : base(context) => _context = context; // Constructor accepting the database context

        public async Task<Case?> GetDetailsAsync(int id) // Retrieve detailed information about a Case by its ID
        {
            return await _context.Cases
                .Include(c => c.Assignees) // Include the assignees 
                .Include(c => c.Evidences) // Include the evidences
                .Include(c => c.Participants) // Include the participants
                .Include(c => c.LinkedReports) // Include the linked reports
                .ThenInclude(l => l.Report) // Include the linked reports and their details
                .Include(c => c.CreatedByUser) // Include the user who created the case
                .FirstOrDefaultAsync(c => c.Id == id); // Find the case by ID with all related data
        }

        public async Task<IEnumerable<Case>> SearchAsync(string? query) // Search for cases based on a query string
        {
            var q = _context.Cases.AsQueryable();
            if (!string.IsNullOrWhiteSpace(query))
                q = q.Where(c => c.Name.Contains(query) || c.Description.Contains(query));
            return await q.Include(c => c.CreatedByUser).ToListAsync();
        }
    }
}
