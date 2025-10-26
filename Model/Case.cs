using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Security.Policy;
using System.Threading.Tasks;

namespace BERihalCodestackerChallenge2025.Model
{
    [Index(nameof(CaseNumber), IsUnique = true)] // Ensure case numbers are unique
    [Index(nameof(Status))] // Index on Status for faster queries
    [Index(nameof(AuthorizationLevel))] // Index on AuthorizationLevel for faster queries
    public class Case // Represents a case in the system
    {
        [Key]
        public int Id { get; set; } // Primary key

        [Required, MaxLength(32)] // Unique case number
        [RegularExpression(@"^[A-Z]{3,6}-\d{4}-\d{1,6}$")] // CASE-2025-001
        public string CaseNumber { get; set; } = default!; // Unique case number

        [Required, MaxLength(160)] // Title of the case
        public string Name { get; set; } = default!; // Short name/title of the case

        [Required, MaxLength(4000)] // Detailed description of the case
        public string Description { get; set; } = default!; // Full description of the case

        [Required, MaxLength(120)] // Location details
        public string AreaCity { get; set; } = default!; // Area or city where the case is being handled

        [Required, MaxLength(80)] // Type of the case
        public string CaseType { get; set; } = "General"; // Type/category of the case

        [Required]
        public Clearance AuthorizationLevel { get; set; } = Clearance.low;

        [Required]
        public CaseStatus Status { get; set; } = CaseStatus.pending;

        [Required]
        public int CreatedByUserId { get; set; }

        [ForeignKey(nameof(CreatedByUserId))]
        public User CreatedByUser { get; set; } = default!;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<CaseAssignee>? Assignees { get; set; }
        public ICollection<Evidence>? Evidences { get; set; }
        public ICollection<CaseParticipant>? Participants { get; set; }
        public ICollection<CaseReport>? LinkedReports { get; set; }
    }
}
