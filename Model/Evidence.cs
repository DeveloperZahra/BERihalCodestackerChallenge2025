using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BERihalCodestackerChallenge2025.Model
{
    // Evidence
    // ---------------------------
    [Index(nameof(CaseId), nameof(IsSoftDeleted))]
    public class Evidence // Represents evidence linked to a case
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CaseId { get; set; }

        [ForeignKey(nameof(CaseId))]
        public Case Case { get; set; } = default!;

        [Required]
        public int AddedByUserId { get; set; }

        [ForeignKey(nameof(AddedByUserId))]
        public User AddedByUser { get; set; } = default!;

        [Required]
        public EvidenceType Type { get; set; }

        // Fluent API CHECK)
        [MaxLength(8000)]
        public string? TextContent { get; set; }

        [MaxLength(1024), Url]
        public string? FileUrl { get; set; }

        [MaxLength(128)]
        public string? MimeType { get; set; }

        public long? SizeBytes { get; set; }

        [Required]
        public bool IsSoftDeleted { get; set; } = false;

        [MaxLength(1024)]
        public string? Remarks { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<EvidenceAuditLog>? Audit { get; set; }
    }
}
