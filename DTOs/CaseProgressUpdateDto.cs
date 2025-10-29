namespace BERihalCodestackerChallenge2025.DTOs
{
    // ================================================================
    // DTO: CaseProgressUpdateDto
    // Description: Used by officers/investigators/admins
    //              to update progress status on an assigned case.
    // ================================================================
    public class CaseProgressUpdateDto
    {
        // e.g., "pending", "ongoing", "closed"
        public string ProgressStatus { get; set; }
    }
}
