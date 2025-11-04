using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BERihalCodestackerChallenge2025.Model;

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

        [MaxLength(1024), Url] // URL to the file, if applicable
        public string? FileUrl { get; set; } // URL to the file, if applicable

        [MaxLength(128)] // File name, if applicable
        public string? MimeType { get; set; } // MIME type of the file, if applicable

        public long? SizeBytes { get; set; } // Size of the file in bytes, if applicable

        [Required]
        public bool IsSoftDeleted { get; set; } = false; // Soft deletion flag

        [MaxLength(1024)]
        public string? Remarks { get; set; } // Additional remarks about the evidence

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Timestamp of when the evidence was added

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; // Timestamp of the last update to the evidence

        public ICollection<EvidenceAuditLog>? Audit { get; set; } // Audit logs related to this evidence
    }
}
