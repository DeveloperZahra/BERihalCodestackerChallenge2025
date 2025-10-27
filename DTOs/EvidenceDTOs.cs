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
}
