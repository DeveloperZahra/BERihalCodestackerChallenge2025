// Repositories/Implementations/ReportRepository.cs
using BERihalCodestackerChallenge2025.Data;
using BERihalCodestackerChallenge2025.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BERihalCodestackerChallenge2025.Repositories
{
    public class ReportRepository : GenericRepository<CrimeReport>, IReportRepository // Report repository implementation
    {
        public ReportRepository(AppDbContext db) : base(db) { } // Constructor accepting the database context

        public Task<CrimeReport?> GetByTrackingCodeAsync(string trackingCode, CancellationToken ct = default) // Retrieve a crime report by its tracking code
            => _db.CrimeReports
                  .Include(r => r.ReportedByUser) // Include the user who reported the crime
                  .FirstOrDefaultAsync(r => r.TrackingCode == trackingCode, ct); // Find the report by tracking code
    }
}
