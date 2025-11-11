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
        public string ClearanceLevel { get; set; } //

    }

    public class CaseAssigneeReadDto
    {
        public int CaseAssigneeId { get; set; }
        public int CaseId { get; set; }
        public int UserId { get; set; }
        public string? UserFullName { get; set; }
        public string? UserRole { get; set; }
        public string ClearanceLevel { get; set; } = default!;
        public string AssignedRole { get; set; } = default!;
        public string ProgressStatus { get; set; } = default!;
        public DateTime AssignedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CaseAssigneeMyAssignmentDto
    {
        public int CaseAssigneeId { get; set; }
        public int CaseId { get; set; }
        public string? CaseName { get; set; }
        public string ProgressStatus { get; set; } = default!;
        public DateTime AssignedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

}
