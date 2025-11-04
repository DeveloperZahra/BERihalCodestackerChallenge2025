using System;

namespace BERihalCodestackerChallenge2025.DTOs
{
    // ============================================================
    // DTO: CaseDetailsDto
    // Purpose: Represent detailed info for a specific criminal case
    // Used in: GET /api/cases/{id}
    // ============================================================
    public class CaseDetailsDto
    {
        // Basic case info
        public int Id { get; set; }
        public string CaseNumber { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string AreaCity { get; set; }
        public string CaseType { get; set; }
        public string Status { get; set; }

        //  Authorization & Level
        public string AuthorizationLevel { get; set; }

        //  Created by info
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        //  Updated info (optional)
        public DateTime? UpdatedAt { get; set; }

        //  Statistics (counts)
        public int NumberOfAssignees { get; set; }
        public int NumberOfEvidences { get; set; }
        public int NumberOfSuspects { get; set; }
        public int NumberOfVictims { get; set; }
        public int NumberOfWitnesses { get; set; }

        //  Optional: reported by citizen/admin/investigator
        public string ReportedBy { get; set; }
    }
}
