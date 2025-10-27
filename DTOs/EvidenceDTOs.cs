namespace BERihalCodestackerChallenge2025.DTOs
{
    // Dto for Evidence create 
    public class EvidenceCreateDto
    {
        public string Type { get; set; } // text | image
        public string? TextContent { get; set; }
        public string? FileUrl { get; set; }
        public string? MimeType { get; set; }
        public long? SizeBytes { get; set; }
        public string? Remarks { get; set; }
    }

    // Dto for Evidence Read 
    public class EvidenceReadDto
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string? TextContent { get; set; }
        public string? FileUrl { get; set; }
        public string? MimeType { get; set; }
        public long? SizeBytes { get; set; }
        public string? Remarks { get; set; }
        public bool IsSoftDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
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

}
