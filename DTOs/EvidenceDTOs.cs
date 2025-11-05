using System.ComponentModel.DataAnnotations;

namespace BERihalCodestackerChallenge2025.DTOs
{
    // Dto for Evidence create 
    public class EvidenceCreateDto
    {
        public int CaseId { get; set; }
        public string Type { get; set; } // text | image
        public string? TextContent { get; set; }
        public string? FileUrl { get; set; }
        public string? MimeType { get; set; }
        public long? SizeBytes { get; set; }
        public string? Remarks { get; set; }

        public int AddedByUserId { get; set; }
    }

    // Dto for Evidence Read 
    public class EvidenceReadDto
    {
        public int Id { get; set; }
        public int CaseId { get; set; }
        public string Type { get; set; }
        public string? TextContent { get; set; }
        public string? FileUrl { get; set; }
        public string? MimeType { get; set; }
        public long? SizeBytes { get; set; }
        public string? Remarks { get; set; }
        public bool IsSoftDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string AddedBy { get; set; }
    }
    // Dto for Evidence Audit to  Represents an audit record of an action taken
    public class EvidenceAuditLogDto
    {
        public string Action { get; set; }
        public string ActedBy { get; set; }
        public DateTime ActedAt { get; set; }
        public string? Details { get; set; }
    }

    // evidance Add image 
    public class EvidenceUpdateImageDto
    {
        [Required, Url, MaxLength(1024)]
        public string FileUrl { get; set; } = default!;

        [Required, MaxLength(128)]
        [RegularExpression(@"^image\/[a-z0-9.+-]+$", ErrorMessage = "MimeType must start with image/.")]
        public string MimeType { get; set; } = default!;

        [Required, Range(1, long.MaxValue, ErrorMessage = "SizeBytes must be > 0")]
        public long? SizeBytes { get; set; }

        [MaxLength(1024)]
        public string? Remarks { get; set; }
        [Required]
        public int ActedByUserId { get; set; }
    }

    public class EvidenceUpdateTextDto
    {
        [Required, MaxLength(5000)]
        public string TextContent { get; set; } = default!;
        [MaxLength(1024)]
        public string? Remarks { get; set; }
        [Required]
        public int ActedByUserId { get; set; }
    }
    

}
