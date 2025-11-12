using BERihalCodestackerChallenge2025.Data;
using BERihalCodestackerChallenge2025.DTOs;
using BERihalCodestackerChallenge2025.Model;
using BERihalCodestackerChallenge2025.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace BERihalCodestackerChallenge2025.Services
{
    public class EvidenceService : IEvidenceService
    {
        private readonly AppDbContext _db;
        private readonly IGenericRepository<Evidence> _evidenceRepo;
        private readonly IGenericRepository<EvidenceAuditLog> _auditRepo;

        private static readonly Regex ImageMime =
            new(@"^image\/[a-z0-9.+-]+$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Dictionary<(int evidenceId, int userId), string> PendingHardDeletes = new();

        public EvidenceService(
            AppDbContext db,
            IGenericRepository<Evidence> evidenceRepo,
            IGenericRepository<EvidenceAuditLog> auditRepo)
        {
            _db = db;
            _evidenceRepo = evidenceRepo;
            _auditRepo = auditRepo;
        }

        // ============================================================
        // Create Text Evidence
        // ============================================================
        public async Task<int> CreateTextAsync(int caseId, int addedByUserId, EvidenceCreateDto dto, CancellationToken ct = default)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

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

            await _evidenceRepo.AddAsync(entity, ct);
            await _db.SaveChangesAsync(ct);

            await _auditRepo.AddAsync(new EvidenceAuditLog
            {
                EvidenceId = entity.Id,
                Action = "add",
                ActedByUserId = addedByUserId,
                ActedBy = $"User ID {addedByUserId}", 
                ActedAt = DateTime.UtcNow,
                Details = "Text evidence created successfully."
            }, ct);

            await _db.SaveChangesAsync(ct);
            return entity.Id;
        }


        // ============================================================
        // Update Image Evidence
        // ============================================================
        public async Task UpdateImageAsync(int evidenceId, EvidenceUpdateImageDto dto, CancellationToken ct = default)
        {
            var e = await _evidenceRepo.GetByIdAsync(evidenceId, ct)
                ?? throw new KeyNotFoundException("Evidence not found.");

            if (e.Type != EvidenceType.image)
                throw new InvalidOperationException("Cannot modify non-image evidence.");

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
                if (dto.SizeBytes <= 0)
                    throw new InvalidOperationException("SizeBytes must be greater than zero.");
                e.SizeBytes = dto.SizeBytes.Value;
            }

            e.Remarks = dto.Remarks;
            e.UpdatedAt = DateTime.UtcNow;

            _evidenceRepo.Update(e);
            await _db.SaveChangesAsync(ct);

            await _auditRepo.AddAsync(new EvidenceAuditLog
            {
                EvidenceId = e.Id,
                Action = "update",
                ActedByUserId = dto.ActedByUserId,
                ActedAt = DateTime.UtcNow,
                Details = "image update"
            }, ct);

            await _db.SaveChangesAsync(ct);
        }

        // ============================================================
        // Get Evidence By Id
        // ============================================================
        public async Task<EvidenceReadDto?> GetAsync(int id, CancellationToken ct = default)
        {
            var e = await _db.Evidences
                .Include(ev => ev.AddedByUser)
                .AsNoTracking()
                .FirstOrDefaultAsync(ev => ev.Id == id, ct);

            if (e == null)
                return null;

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
                AddedBy = e.AddedByUser?.Username,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt,
                IsSoftDeleted = e.IsSoftDeleted
            };
        }

        // ============================================================
        // Get Image Data
        // ============================================================
        public async Task<(byte[] Bytes, string Mime)> GetImageAsync(int id, CancellationToken ct = default)
        {
            var evidence = await _db.Evidences
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id && e.Type == EvidenceType.image, ct);

            if (evidence == null || evidence.ImageData == null)
                return (Array.Empty<byte>(), string.Empty);

            return (evidence.ImageData, evidence.MimeType ?? "application/octet-stream");
        }

        // ============================================================
        // Update Text Evidence
        // ============================================================
        public async Task UpdateTextAsync(int id, EvidenceUpdateTextDto dto, CancellationToken ct = default)
        {
            var e = await _evidenceRepo.GetByIdAsync(id, ct)
                ?? throw new KeyNotFoundException("Evidence not found.");

            if (e.Type != EvidenceType.text)
                throw new InvalidOperationException("Cannot modify non-text evidence.");

            e.TextContent = dto.TextContent;
            e.Remarks = dto.Remarks;
            e.UpdatedAt = DateTime.UtcNow;

            _evidenceRepo.Update(e);
            await _db.SaveChangesAsync(ct);

            await _auditRepo.AddAsync(new EvidenceAuditLog
            {
                EvidenceId = e.Id,
                Action = "update",
                ActedByUserId = dto.ActedByUserId,
                ActedAt = DateTime.UtcNow,
                Details = "text update"
            }, ct);

            await _db.SaveChangesAsync(ct);
        }

        // ============================================================
        // Soft Delete
        // ============================================================
        public async Task SoftDeleteAsync(int id, int actedByUserId, string? reason = null, CancellationToken ct = default)
        {
            var e = await _evidenceRepo.GetByIdAsync(id, ct)
                ?? throw new KeyNotFoundException("Evidence not found.");

            e.IsSoftDeleted = true;
            e.Remarks = reason ?? "Soft deleted.";
            e.UpdatedAt = DateTime.UtcNow;

            _evidenceRepo.Update(e);
            await _db.SaveChangesAsync(ct);

            await _auditRepo.AddAsync(new EvidenceAuditLog
            {
                EvidenceId = e.Id,
                Action = "soft_delete",
                ActedByUserId = actedByUserId,
                ActedAt = DateTime.UtcNow,
                Details = reason ?? "Soft delete action"
            }, ct);

            await _db.SaveChangesAsync(ct);
        }

        // ============================================================
        // Hard Delete (3-step confirmation)
        // ============================================================
        public Task<string> StartHardDeleteAsync(int id, int actedByUserId)
        {
            PendingHardDeletes[(id, actedByUserId)] = "started";
            return Task.FromResult($"Are you sure you want to permanently delete Evidence ID: {id}? (yes/no)");
        }

        public Task<string> ConfirmHardDeleteAsync(int id, int actedByUserId, string yesNo)
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
            //  Step 1: Validate command format
            string expectedCommand = $"DELETE {id}";
            if (!string.Equals(command?.Trim(), expectedCommand, StringComparison.OrdinalIgnoreCase))
                return false; // invalid or unconfirmed command

            //  Step 2: Check if evidence exists
            var evidence = await _db.Evidences
                .Include(e => e.AddedByUser)
                .FirstOrDefaultAsync(e => e.Id == id, ct);

            if (evidence == null)
                throw new KeyNotFoundException($"Evidence with ID {id} not found.");

            //  Step 3: Find related audit logs (FK references)
            var auditLogs = await _db.EvidenceAuditLogs
                .Where(a => a.EvidenceId == id)
                .ToListAsync(ct);

            //  Step 4: Delete related audit logs FIRST
            if (auditLogs.Any())
            {
                _db.EvidenceAuditLogs.RemoveRange(auditLogs);
                await _db.SaveChangesAsync(ct);
            }

            //  Step 5: Remove the evidence itself
            _db.Evidences.Remove(evidence);
            await _db.SaveChangesAsync(ct);

            //  Step 6: Log the hard delete action
            var log = new EvidenceAuditLog
            {
                EvidenceId = id,
                Action = "Hard Delete",
                ActedBy = $"User ID {actedByUserId}",
                ActedAt = DateTime.UtcNow,
                Details = $"Evidence ID {id} was permanently deleted by User ID {actedByUserId}."
            };

            _db.EvidenceAuditLogs.Add(log);
            await _db.SaveChangesAsync(ct);

            return true;
        }

    }
}
