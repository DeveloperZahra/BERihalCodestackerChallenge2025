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
        public int Id { get; set; }

        [Required]
        public int CaseId { get; set; }

        [ForeignKey(nameof(CaseId))]
        public Case Case { get; set; } = default!;

        [Required]
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = default!;

        //
        [Required, MaxLength(32)]
        public string AssignedRole { get; set; } = "Officer"; // "Investigator" 

        //
        [Required]
        public CaseStatus ProgressStatus { get; set; } = CaseStatus.pending;

        [Required]
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    }

}
