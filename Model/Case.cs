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
    public class Case
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(32)]
        [RegularExpression(@"^[A-Z]{3,6}-\d{4}-\d{1,6}$")] // CASE-2025-001
        public string CaseNumber { get; set; } = default!;

        [Required, MaxLength(160)]
        public string Name { get; set; } = default!;

        [Required, MaxLength(4000)]
        public string Description { get; set; } = default!;

        [Required, MaxLength(120)]
        public string AreaCity { get; set; } = default!;

        [Required, MaxLength(80)]
        public string CaseType { get; set; } = "General";

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
