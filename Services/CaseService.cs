using BERihalCodestackerChallenge2025.DTOs;
using BERihalCodestackerChallenge2025.Model;
using BERihalCodestackerChallenge2025.Repositories;

namespace BERihalCodestackerChallenge2025.Services
{
    public class CaseService : ICaseService 
    {
        private readonly IUnitOfWork _uow; 
        private readonly IGenericRepository<Case> _cases; 

        public CaseService(IUnitOfWork uow, IGenericRepository<Case> casesRepo) 
        {
            _uow = uow;
            _cases = casesRepo;
        }

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
            await _uow.SaveChangesAsync(ct);         // 

            return (entity.CaseId, entity.CaseNumber);
        }

         
    }
}



