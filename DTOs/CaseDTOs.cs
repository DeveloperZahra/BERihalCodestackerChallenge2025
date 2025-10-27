namespace BERihalCodestackerChallenge2025.DTOs
{
    // DTO to create a new case
    public class CaseCreateDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string AreaCity { get; set; }
        public string CaseType { get; set; }
        public string AuthorizationLevel { get; set; } // low | medium | high | critical
        public List<int>? ReportIds { get; set; } // link existing crime reports
    }


}
