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
        public int Id { get; set; }

        [Required]
        public int EvidenceId { get; set; }

        [ForeignKey(nameof(EvidenceId))]
        public Evidence Evidence { get; set; } = default!;

        [Required, MaxLength(32)]
        [RegularExpression("^(add|update|soft_delete|hard_delete)$")]
        public string Action { get; set; } = "add";

        [Required]
        public int ActedByUserId { get; set; }

        [ForeignKey(nameof(ActedByUserId))]
        public User ActedByUser { get; set; } = default!;

        [Required]
        public DateTime ActedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(2000)]
        public string? Details { get; set; }
    }

}
