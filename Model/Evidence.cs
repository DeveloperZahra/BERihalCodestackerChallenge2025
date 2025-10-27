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
        public int Id { get; set; } // Primary key

        [Required]
        public int CaseId { get; set; } // Foreign key to the associated case

        [ForeignKey(nameof(CaseId))]
        public Case Case { get; set; } = default!; // Navigation property to the associated case

        [Required]
        public int AddedByUserId { get; set; } // Foreign key to the user who added the evidence

        [ForeignKey(nameof(AddedByUserId))] // Navigation property to the user who added the evidence
        public User AddedByUser { get; set; } = default!; // Navigation property to the user who added the evidence

        [Required]
        public EvidenceType Type { get; set; } // Type of the evidence (e.g., Text, File)

        // Fluent API CHECK)
        [MaxLength(8000)]
        public string? TextContent { get; set; } // Text content of the evidence, if applicable

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
