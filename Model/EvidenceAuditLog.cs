using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BERihalCodestackerChallenge2025.Model
{
    // Evidence Audit Logs
    // ---------------------------
    [Index(nameof(EvidenceId))]
    public class EvidenceAuditLog // Represents an audit log entry for actions taken on evidence
    {
        [Key]
        public int Id { get; set; } // Primary key

        [ForeignKey(nameof(EvidenceId))] // Navigation property to the associated evidence
        [Required]
        public int EvidenceId { get; set; } // Foreign key to the associated evidence

        
        public Evidence Evidence { get; set; } = default!; // Navigation property to the associated evidence

        [Required, MaxLength(32)]
        [RegularExpression("^(add|update|soft_delete|hard_delete)$")] // Action performed on the evidence
        public string Action { get; set; } = "add"; // Action performed on the evidence

        [ForeignKey(nameof(ActedByUserId))]
        [Required]
        public int ActedByUserId { get; set; } // Foreign key to the user who performed the action

        public User ActedByUser { get; set; } = default!; // Navigation property to the user who performed the action

        [Required]
        public DateTime ActedAt { get; set; } = DateTime.UtcNow; // Timestamp of when the action was performed

        [MaxLength(2000)]
        public string? Details { get; set; } // Additional details about the action
    }

}
