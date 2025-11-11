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
                CaseNumber = $"CASE-{DateTime.UtcNow:yyyy}-{Random.Shared.Next(1, 999999):000000}",
                Name = dto.Name,
                Description = dto.Description,
                AreaCity = dto.AreaCity,
                CaseType = dto.CaseType,
                AuthorizationLevel = level,
                Status = CaseStatus.pending,
                CreatedByUserId = createdByUserId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _cases.AddAsync(entity, ct);
            await _db.SaveChangesAsync(ct);

            return (entity.CaseId, entity.CaseNumber);
        }
        // ==========================
        // Get all cases with optional search query
        // ==========================
        public async Task<IEnumerable<CaseReadDto>> GetAllAsync(CancellationToken ct)
        {
            return await _db.Cases
                .AsNoTracking()
                .Include(c => c.CreatedByUser)
                .Include(c => c.CrimeReports)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new CaseReadDto
                {
                    CaseId = c.CaseId,
                    Name = c.Name,
                    Description = c.Description,
                    CreatedAt = c.CreatedAt,
                    CreatedByUserName = c.CreatedByUser.FullName,
                    CrimeReportsCount = c.CrimeReports.Count
                })
                .ToListAsync(ct);
        }

        // ==========================================================
        //  Get Case By ID
        // ==========================================================
        public async Task<CaseListItemDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var entity = await _db.Cases
                                  .Include(c => c.CreatedByUser)
                                  .AsNoTracking()
                                  .FirstOrDefaultAsync(c => c.CaseId == id, ct);

            if (entity == null)
                return null;

            return new CaseListItemDto
            {
                CaseNumber = entity.CaseNumber,
                Name = entity.Name,
                Description = entity.Description,
                AreaCity = entity.AreaCity,
                CreatedBy = entity.CreatedByUser.Username,
                CreatedAt = entity.CreatedAt,
                CaseType = entity.CaseType,
                AuthorizationLevel = entity.AuthorizationLevel.ToString()
            };
        }

        // ==========================================================
        //  Update Case 
        // ==========================================================
        public async Task<CaseListItemDto?> UpdateAsync(int id, CaseUpdateDto dto, CancellationToken ct = default)
        {
            var entity = await _cases.GetByIdAsync(id, ct);
            if (entity == null)
                return null;

            //  Update fields only if provided
            entity.Name = dto.Name ?? entity.Name;
            entity.Description = dto.Description ?? entity.Description;
            entity.AreaCity = dto.AreaCity ?? entity.AreaCity;
            entity.CaseType = dto.CaseType ?? entity.CaseType;

            if (!string.IsNullOrWhiteSpace(dto.AuthorizationLevel))
            {
                if (!Enum.TryParse<Clearance>(dto.AuthorizationLevel, true, out var level))
                    throw new ArgumentException("Invalid AuthorizationLevel value.");
                entity.AuthorizationLevel = level;
            }

            if (!string.IsNullOrWhiteSpace(dto.Status))
            {
                if (!Enum.TryParse<CaseStatus>(dto.Status, true, out var status))
                    throw new ArgumentException("Invalid CaseStatus value.");
                entity.Status = status;
            }

            entity.UpdatedAt = DateTime.UtcNow;

            _cases.Update(entity);
            await _db.SaveChangesAsync(ct);

            return new CaseListItemDto
            {
                CaseNumber = entity.CaseNumber,
                Name = entity.Name,
                Description = entity.Description,
                AreaCity = entity.AreaCity,
                CreatedBy = (await _db.Users.FindAsync(entity.CreatedByUserId))?.Username ?? "Unknown",
                CreatedAt = entity.CreatedAt,
                CaseType = entity.CaseType,
                AuthorizationLevel = entity.AuthorizationLevel.ToString()
            };
        }

        // ==========================================================
        //  Delete Case
        // ==========================================================
        public async Task<bool> DeleteAsync(int id, CancellationToken ct)
        {
            var entity = await _cases.GetByIdAsync(id, ct);
            if (entity == null)
                return false;

            _cases.Delete(entity);
            await _db.SaveChangesAsync(ct);
            return true;
        }

        //==========================================================
        // Helper Method
        //==========================================================
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



    }
}



