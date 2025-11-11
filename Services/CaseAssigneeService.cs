using BERihalCodestackerChallenge2025.Data;
using BERihalCodestackerChallenge2025.DTOs;
using BERihalCodestackerChallenge2025.Model;
using BERihalCodestackerChallenge2025.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BERihalCodestackerChallenge2025.Services
{
    public class CaseAssigneeService : ICaseAssigneeService
    {
        private readonly AppDbContext _db;
        private readonly ICaseAssigneeRepository _assignees;

        public CaseAssigneeService(AppDbContext db, ICaseAssigneeRepository assignees)
        {
            _db = db;
            _assignees = assignees;
        }

        // Assign user to case
        public async Task<CaseAssigneeReadDto> AssignAsync(CaseAssigneeCreateDto dto, CancellationToken ct = default)
        {
            if (dto is null) throw new ArgumentNullException(nameof(dto));
            if (dto.CaseId <= 0 || dto.UserId <= 0)
                throw new ArgumentException("CaseId and UserId must be positive.");

            // ensure case & user exist
            var caseExists = await _db.Cases.AsNoTracking().AnyAsync(c => c.CaseId == dto.CaseId, ct);
            if (!caseExists) throw new KeyNotFoundException($"Case {dto.CaseId} not found.");

            var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == dto.UserId, ct);
            if (user is null) throw new KeyNotFoundException($"User {dto.UserId} not found.");

            // prevent duplicates (unique index exists too)
            if (await _assignees.ExistsAsync(dto.CaseId, dto.UserId, ct))
                throw new InvalidOperationException("User is already assigned to this case.");

            // validate clearance (string but must match enum values)
            if (!Enum.TryParse<Clearance>(dto.ClearanceLevel, true, out _))
                throw new ArgumentException($"Invalid ClearanceLevel. Allowed: {string.Join(", ", Enum.GetNames(typeof(Clearance)))}");

            var entity = new CaseAssignee
            {
                CaseId = dto.CaseId,
                UserId = dto.UserId,
                ClearanceLevel = dto.ClearanceLevel,
                AssignedRole = "Officer",
                ProgressStatus = CaseStatus.pending,
                AssignedAt = DateTime.UtcNow,
                UpdatedAt = null
            };

            //save — We use DbContext directly to avoid GenericRepository interface differences
            _db.CaseAssignees.Add(entity);
            await _db.SaveChangesAsync(ct);

            // map to read dto (lightweight projection)
            return new CaseAssigneeReadDto
            {
                CaseAssigneeId = entity.CaseAssigneeId,
                CaseId = entity.CaseId,
                UserId = entity.UserId,
                ClearanceLevel = entity.ClearanceLevel,
                AssignedRole = entity.AssignedRole,
                ProgressStatus = entity.ProgressStatus.ToString(),
                AssignedAt = entity.AssignedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }

        // Get all assignees for a case
        public async Task<IReadOnlyList<CaseAssigneeReadDto>> GetByCaseAsync(int caseId, CancellationToken ct = default)
        {
            if (caseId <= 0) throw new ArgumentException("Invalid caseId.");

            var caseExists = await _db.Cases.AsNoTracking().AnyAsync(c => c.CaseId == caseId, ct);
            if (!caseExists) throw new KeyNotFoundException($"Case {caseId} not found.");

            var items = await _assignees.GetByCaseAsync(caseId, ct);

            return items
                .Select(a => new CaseAssigneeReadDto
                {
                    CaseAssigneeId = a.CaseAssigneeId,
                    CaseId = a.CaseId,
                    UserId = a.UserId,
                    UserFullName = a.User?.FullName,
                    UserRole = a.User?.Role.ToString(),
                    ClearanceLevel = a.ClearanceLevel,
                    AssignedRole = a.AssignedRole,
                    ProgressStatus = a.ProgressStatus.ToString(),
                    AssignedAt = a.AssignedAt,
                    UpdatedAt = a.UpdatedAt
                })
                .ToList();
        }

        // Update progress status
        public async Task<CaseAssigneeReadDto?> UpdateProgressAsync(int assigneeId, CaseProgressUpdateDto dto, CancellationToken ct = default)
        {
            if (assigneeId <= 0) throw new ArgumentException("Invalid assigneeId.");
            if (dto is null) throw new ArgumentNullException(nameof(dto));
            if (!Enum.TryParse<CaseStatus>(dto.ProgressStatus, true, out var newStatus))
                throw new ArgumentException($"Invalid ProgressStatus. Allowed: {string.Join(", ", Enum.GetNames(typeof(CaseStatus)))}");

            var entity = await _db.CaseAssignees
                .Include(a => a.User)
                .Include(a => a.Case)
                .FirstOrDefaultAsync(a => a.CaseAssigneeId == assigneeId, ct);

            if (entity is null) return null;

            entity.ProgressStatus = newStatus;
            entity.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync(ct);

            return new CaseAssigneeReadDto
            {
                CaseAssigneeId = entity.CaseAssigneeId,
                CaseId = entity.CaseId,
                UserId = entity.UserId,
                UserFullName = entity.User?.FullName,
                UserRole = entity.User?.Role.ToString(),
                ClearanceLevel = entity.ClearanceLevel,
                AssignedRole = entity.AssignedRole,
                ProgressStatus = entity.ProgressStatus.ToString(),
                AssignedAt = entity.AssignedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }

        // Remove assignee
        public async Task<bool> RemoveAsync(int assigneeId, CancellationToken ct = default)
        {
            if (assigneeId <= 0) throw new ArgumentException("Invalid assigneeId.");

            var entity = await _db.CaseAssignees.FirstOrDefaultAsync(a => a.CaseAssigneeId == assigneeId, ct);
            if (entity is null) return false;

            _db.CaseAssignees.Remove(entity);
            await _db.SaveChangesAsync(ct);
            return true;
        }

        // Get assignments of a specific user
        public async Task<IReadOnlyList<CaseAssigneeMyAssignmentDto>> GetMyAssignmentsAsync(int userId, CancellationToken ct = default)
        {
            if (userId <= 0) throw new ArgumentException("Invalid userId.");

            var list = await _assignees.GetMyAssignmentsAsync(userId, ct);
            return list.Select(a => new CaseAssigneeMyAssignmentDto
            {
                CaseAssigneeId = a.CaseAssigneeId,
                CaseId = a.CaseId,
                CaseName = a.Case?.Name,
                ProgressStatus = a.ProgressStatus.ToString(),
                AssignedAt = a.AssignedAt,
                UpdatedAt = a.UpdatedAt
            }).ToList();
        }
    }
}
