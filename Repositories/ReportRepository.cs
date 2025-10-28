using BERihalCodestackerChallenge2025.Data;
using BERihalCodestackerChallenge2025.Model;
using Microsoft.EntityFrameworkCore;

namespace BERihalCodestackerChallenge2025.Repositories
{
    public class ReportRepository : GenericRepository<CrimeReport>, IReportRepository // Repository for crime reports
    {
        private readonly AppDbContext _context;
        public ReportRepository(AppDbContext context) : base(context) => _context = context;

        public async Task<CrimeReport?> GetByTrackingCodeAsync(string trackingCode)
        {
            return await _context.CrimeReports
                .Include(r => r.ReportedByUser)
                .FirstOrDefaultAsync(r => r.TrackingCode == trackingCode);
        }
    }
}
