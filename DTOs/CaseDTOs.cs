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

    // DTO to update existing case
    public class CaseUpdateDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? AreaCity { get; set; }
        public string? CaseType { get; set; }
        public string? AuthorizationLevel { get; set; }
        public string? Status { get; set; } // pending | ongoing | closed
    }

    // DTO for list API (with smart trimmed description)
    public class CaseListItemDto
    {
        public string CaseNumber { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } // ≤100 chars (trimmed)
        public string AreaCity { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CaseType { get; set; }
        public string AuthorizationLevel { get; set; }
    }

    // DTO for case assignees
    public class CaseAssigneeDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string AssignedRole { get; set; }
        public string ProgressStatus { get; set; }
        public DateTime AssignedAt { get; set; }
    }

    public class CaseReadDto
    {
        public int CaseId { get; set; }              
        public string Name { get; set; } = string.Empty; 
        public string? Description { get; set; }       
        public DateTime CreatedAt { get; set; }          
        public string CreatedByUserName { get; set; } = string.Empty; 
        public int CrimeReportsCount { get; set; }       
    }


}
