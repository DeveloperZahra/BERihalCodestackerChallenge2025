namespace BERihalCodestackerChallenge2025.DTOs
{
    // ================================================================
    // DTO: CaseAssigneeCreateDto
    // Description: Used when assigning a user (officer/investigator)
    //              to a case by Admin or Investigator.
    // ================================================================
    public class CaseAssigneeCreateDto
    {
        public int CaseId { get; set; }     //  ID of the case
        public int UserId { get; set; }     //  ID of the assigned user
    }
}
