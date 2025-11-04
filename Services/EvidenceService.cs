using System.Text.RegularExpressions;
using BERihalCodestackerChallenge2025.DTOs;
using BERihalCodestackerChallenge2025.Model;
using BERihalCodestackerChallenge2025.Repositories;

namespace BERihalCodestackerChallenge2025.Services
{
    public class EvidenceService : IEvidenceService
    {
        private readonly IUnitOfWork _uow;
        private readonly IGenericRepository<Evidence> _EvidencegenericRepository;
        private readonly IGenericRepository<EvidenceAuditLog> _EvidenceAuditLoggenericRepository;


        private static readonly Regex ImageMime = new(@"^image\/[a-z0-9.+-]+$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        
        private static readonly Dictionary<(int evidenceId, int userId), string> PendingHardDeletes = new();

            _uow = uow; 

            _EvidencegenericRepository = EvidencegenericRepository; 
            _EvidenceAuditLoggenericRepository = EvidenceAuditLoggenericRepository; 
        }

        public async Task<int> CreateTextAsync(int caseId, int addedByUserId, EvidenceCreateDto dto, CancellationToken ct = default)
        {
            var entity = new Evidence
            {
                CaseId = caseId,
                AddedByUserId = addedByUserId,
                Type = EvidenceType.text,
                TextContent = dto.TextContent,
                Remarks = dto.Remarks,
                IsSoftDeleted = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _EvidencegenericRepository.AddAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);


            var auditLogEntity = new EvidenceAuditLog
            {
                EvidenceId = entity.Id,
                Action = "add",
                ActedByUserId = addedByUserId,
                ActedAt = DateTime.UtcNow,
                Details = "text"
            };

            await _EvidenceAuditLoggenericRepository.AddAsync(auditLogEntity, ct);
            await _uow.SaveChangesAsync(ct);

            return entity.Id;
        }


        public async Task UpdateImageAsync(int evidenceId, EvidenceUpdateImageDto dto, CancellationToken ct = default)
        {
            var e = await _EvidencegenericRepository.GetByIdAsync(evidenceId, ct)
                    ?? throw new KeyNotFoundException("Evidence not found");

            if (e.Type != EvidenceType.image)
                throw new InvalidOperationException("Cannot change evidence type.");

            if (!string.IsNullOrWhiteSpace(dto.MimeType))
            {
                if (!ImageMime.IsMatch(dto.MimeType))
                    throw new InvalidOperationException("Invalid image mime type.");
                e.MimeType = dto.MimeType;
            }

            if (!string.IsNullOrWhiteSpace(dto.FileUrl))
                e.FileUrl = dto.FileUrl;

            if (dto.SizeBytes.HasValue)
            {
                if (dto.SizeBytes.Value <= 0)
                    throw new InvalidOperationException("SizeBytes must be > 0.");
                e.SizeBytes = dto.SizeBytes.Value;
            }

            e.Remarks = dto.Remarks;
            e.UpdatedAt = DateTime.UtcNow;

            _EvidencegenericRepository.Update(e);
            await _uow.SaveChangesAsync(ct);

            await _EvidenceAuditLoggenericRepository.AddAsync(new EvidenceAuditLog
            {
                EvidenceId = e.Id,
                Action = "update",
                ActedByUserId = dto.ActedByUserId,
                ActedAt = DateTime.UtcNow,
                Details = "image update"
            }, ct);
            await _uow.SaveChangesAsync(ct);
        }
        public async Task<EvidenceReadDto?> GetAsync(int id, CancellationToken ct = default)
        {
            var e = await _uow.Evidence.GetWithUserAsync(id, ct);
            if (e is null) return null;

            return new EvidenceReadDto
            {
                Id = e.Id,
                CaseId = e.CaseId,
                Type = e.Type.ToString(),
                TextContent = e.TextContent,
                FileUrl = e.FileUrl,
                MimeType = e.MimeType,
                SizeBytes = e.SizeBytes,
                Remarks = e.Remarks,
                AddedBy = e.AddedByUser.Username,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt,
                IsSoftDeleted = e.IsSoftDeleted
            };
        }

        public async Task<(byte[] bytes, string mime)?> GetImageAsync(int id, CancellationToken ct = default)
        {
            var e = await _EvidencegenericRepository.GetByIdAsync(id, ct);
            if (e is null || e.Type != EvidenceType.image || string.IsNullOrWhiteSpace(e.FileUrl) || string.IsNullOrWhiteSpace(e.MimeType))
                return null;

            
            return (Array.Empty<byte>(), e.MimeType);
        }

        public async Task UpdateTextAsync(int id, EvidenceUpdateTextDto dto, CancellationToken ct = default)
        {
            var e = await _EvidencegenericRepository.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException("Evidence not found");
            if (e.Type != EvidenceType.text) throw new InvalidOperationException("Cannot change evidence type.");
            e.TextContent = dto.TextContent;
            e.Remarks = dto.Remarks;
            e.UpdatedAt = DateTime.UtcNow;

            _EvidencegenericRepository.Update(e);
            await _uow.SaveChangesAsync(ct);

            await _EvidenceAuditLoggenericRepository.AddAsync(new EvidenceAuditLog
            {
                EvidenceId = e.Id,
                Action = "update",
                ActedByUserId = dto.ActedByUserId,
                ActedAt = DateTime.UtcNow,
                Details = "text update"
            }, ct);
            await _uow.SaveChangesAsync(ct);
        }

        

        public async Task SoftDeleteAsync(int id, int actedByUserId, string? reason = null, CancellationToken ct = default)
        {
            var e = await _EvidencegenericRepository.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException("Evidence not found");
            await _uow.Evidence.SoftDeleteAsync(e, actedByUserId, reason, ct);
            await _uow.SaveChangesAsync(ct);
        }

        public Task<string> StartHardDeleteAsync(int id, int actedByUserId, CancellationToken ct = default)
        {
            PendingHardDeletes[(id, actedByUserId)] = "started";
            return Task.FromResult($"Are you sure you want to permanently delete Evidence ID: {id}? (yes/no)");
        }

        public Task<string> ConfirmHardDeleteAsync(int id, int actedByUserId, string yesNo, CancellationToken ct = default)
        {
            if (!PendingHardDeletes.TryGetValue((id, actedByUserId), out _))
                return Task.FromResult("No deletion session started.");
            if (!string.Equals(yesNo, "yes", StringComparison.OrdinalIgnoreCase))
            {
                PendingHardDeletes.Remove((id, actedByUserId));
                return Task.FromResult("Deletion canceled.");
            }
            PendingHardDeletes[(id, actedByUserId)] = "confirmed";
            return Task.FromResult($"Send: DELETE {id}");
        }

        public async Task<bool> FinalizeHardDeleteAsync(int id, int actedByUserId, string command, CancellationToken ct = default)
        {
            if (!PendingHardDeletes.TryGetValue((id, actedByUserId), out var state) || state != "confirmed")
                return false;

            if (!string.Equals(command, $"DELETE {id}", StringComparison.OrdinalIgnoreCase))
                return false;

            var e = await _EvidencegenericRepository.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException("Evidence not found");
            await _uow.Evidence.HardDeleteAsync(e, actedByUserId, "hard delete confirmed", ct);
            await _uow.SaveChangesAsync(ct);

            PendingHardDeletes.Remove((id, actedByUserId));
            return true;
        }
    }
}
