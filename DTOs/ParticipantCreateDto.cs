namespace BERihalCodestackerChallenge2025.DTOs
{
    // Dto for participant create 
    public class ParticipantCreateDto
    {
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string? Notes { get; set; }
    }

    // Dto for Case Participant Create
    public class CaseParticipantCreateDto
    {
        public int CaseId { get; set; }
        public int? ParticipantId { get; set; } // existing participant (optional)
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? Notes { get; set; }
        public string Role { get; set; } // suspect | victim | witness
    }

    // Dto for Case Participant Read
    public class CaseParticipantReadDto
    {
        public int ParticipantId { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Role { get; set; }
        public string? AddedBy { get; set; }
        public DateTime AddedAt { get; set; }
    }

}
