using BERihalCodestackerChallenge2025.Data;
using BERihalCodestackerChallenge2025.DTOs;
using BERihalCodestackerChallenge2025.Model;
using BERihalCodestackerChallenge2025.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BERihalCodestackerChallenge2025.Services
{
    public class CaseService : ICaseService 
    {
        private readonly AppDbContext _db;
        //  private readonly IUnitOfWork _uow; 
        private readonly IGenericRepository<Case> _cases; 

        public CaseService(AppDbContext db, IGenericRepository<Case> casesRepo) 
        {
          //  _uow = uow;
            _cases = casesRepo;
            _db = db;
        }
        //==========================
        //    create new case
        //==========================
        public async Task<(int caseId, string caseNumber)> CreateAsync(
            int createdByUserId, CaseCreateDto dto, CancellationToken ct = default)
        {
            if (!Enum.TryParse<Clearance>(dto.AuthorizationLevel, true, out var level))
                throw new ArgumentException("Invalid AuthorizationLevel. Allowed: low, medium, high, critical.");

            var entity = new Case
            {
                CaseNumber = $"CASE-{DateTime.UtcNow:yyyy}-{Random.Shared.Next(1, 999999):000000}", //
                Name = dto.Name,
                Description = dto.Description,
                AreaCity = dto.AreaCity,
                CaseType = dto.CaseType,
                AuthorizationLevel = level,
                Status = CaseStatus.pending,
                CreatedByUserId = createdByUserId,
                CreatedAt = DateTime.UtcNow
            };

            await _cases.AddAsync(entity, ct);        
            await _db.SaveChangesAsync(ct);         // 

            return (entity.CaseId, entity.CaseNumber);
        }
        // ==========================
        // Get all cases with optional search query
        // ==========================
        public async Task<IEnumerable<CaseListItemDto>> GetAllAsync(string? q, CancellationToken ct = default)
        {
            
            var query = _db.Cases
                .AsNoTracking() 
                .Include(c => c.CreatedByUser)
                .AsQueryable();

           
            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim().ToLower();
                query = query.Where(c =>
                    EF.Functions.Like(c.Name.ToLower(), $"%{term}%") ||
                    EF.Functions.Like(c.Description.ToLower(), $"%{term}%"));
            }

            var list = await query.Select(c => new CaseListItemDto
            {
                CaseNumber = c.CaseNumber,
                Name = c.Name,
                Description = TruncateWithWordBoundary(c.Description, 100),
                AreaCity = c.AreaCity,
                CreatedBy = c.CreatedByUser.Username,
                CreatedAt = c.CreatedAt,
                CaseType = c.CaseType,
                AuthorizationLevel = c.AuthorizationLevel.ToString()
            }).ToListAsync(ct);

            return list;
        }

        // ==========================
        // Helper method to truncate description text
        // ==========================
        private static string TruncateWithWordBoundary(string? text, int max)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            if (text.Length <= max)
                return text;

            var slice = text[..max];
            var lastSpace = slice.LastIndexOf(' ');

            return (lastSpace > 0 ? slice[..lastSpace] : slice) + " ...";
        }

        //==========================
        //       delete case
        //==========================
        public async Task<bool> DeleteAsync(int id, CancellationToken ct)
        {
            var entity = await _cases.GetByIdAsync(id, ct);
            if (entity == null) return false;

            _cases.Delete(entity);
            await _db.SaveChangesAsync(ct);
            return true;
        }

    }
}



