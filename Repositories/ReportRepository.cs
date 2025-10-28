using BERihalCodestackerChallenge2025.Data;
using BERihalCodestackerChallenge2025.Model;
using Microsoft.EntityFrameworkCore;

namespace BERihalCodestackerChallenge2025.Repositories
{
    public class ReportRepository : GenericRepository<CrimeReport>, IReportRepository // Repository for crime reports
    {
        private readonly AppDbContext _context; // Database context
        public ReportRepository(AppDbContext context) : base(context) => _context = context; // Constructor accepting the database context

        public async Task<CrimeReport?> GetByTrackingCodeAsync(string trackingCode) // Retrieve a crime report by its tracking code
        {
            return await _context.CrimeReports 
                .Include(r => r.ReportedByUser) // Include the user who reported the crime
                .FirstOrDefaultAsync(r => r.TrackingCode == trackingCode);
        }
    }
}
