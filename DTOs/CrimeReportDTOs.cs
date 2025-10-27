namespace BERihalCodestackerChallenge2025.DTOs
{
    // Public submission DTO (Citizen or Registered User)
    public class CrimeReportCreateDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string AreaCity { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public int? ReportedByUserId { get; set; } // null => Citizen
    }
}
