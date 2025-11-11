using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace BERihalCodestackerChallenge2025.Model
{
    [Index(nameof(CaseId), nameof(UserId), IsUnique = true)] // Ensure a user is assigned only once per case
    public class CaseAssignee
    {
        [Key]
        public int CaseAssigneeId { get; set; } // Primary key

        [Required]
        public int CaseId { get; set; } // Foreign key to the case

        [ForeignKey(nameof(CaseId))] // Navigation property to the case
        public Case Case { get; set; } = default!; // Navigation property to the case

        [Required]
        public int UserId { get; set; } // Foreign key to the user

        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = default!; // Navigation property to the user

        //
        [Required, MaxLength(32)]
        public string AssignedRole { get; set; } = "Officer"; // Role assigned to the user for this case 

        //
        [Required]
        public CaseStatus ProgressStatus { get; set; } = CaseStatus.pending; // Progress status of the assignee in the case

        [Required]
        public string ClearanceLevel { get; set; } //Determines the level of access to the case


        [Required]
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow; // Timestamp of when the user was assigned to the case

        [Required]
        public DateTime? UpdatedAt { get; set; } // Nullable to allow null for new records
    }

}
