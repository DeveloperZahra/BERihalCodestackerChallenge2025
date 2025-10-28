using BERihalCodestackerChallenge2025.Data;
using BERihalCodestackerChallenge2025.Model;
using Microsoft.EntityFrameworkCore;

namespace BERihalCodestackerChallenge2025.Repositories
{
    public class CaseRepository : GenericRepository<Case>, ICaseRepository // Repository for Case entity with specific methods
    {
        private readonly AppDbContext _context;
        public CaseRepository(AppDbContext context) : base(context) => _context = context;

        public async Task<Case?> GetDetailsAsync(int id)
        {
            return await _context.Cases
                .Include(c => c.Assignees)
                .Include(c => c.Evidences)
                .Include(c => c.Participants)
                .Include(c => c.LinkedReports)
                .ThenInclude(l => l.Report)
                .Include(c => c.CreatedByUser)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Case>> SearchAsync(string? query)
        {
            var q = _context.Cases.AsQueryable();
            if (!string.IsNullOrWhiteSpace(query))
                q = q.Where(c => c.Name.Contains(query) || c.Description.Contains(query));
            return await q.Include(c => c.CreatedByUser).ToListAsync();
        }
    }
}
