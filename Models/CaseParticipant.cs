using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BERihalCodestackerChallenge2025.Model
{
    // Junction: Case <-> Participants
  
    [Index(nameof(CaseId), nameof(ParticipantId), nameof(Role), IsUnique = false)] // A participant can have multiple roles in a case
    public class CaseParticipant // Represents a participant involved in a case
    {
        [Key]
        public int Id { get; set; } // Primary key

        [Required]
        public int CaseId { get; set; } // Foreign key to the case

        [ForeignKey(nameof(CaseId))]
        public Case Case { get; set; } = default!; // Navigation property to the case

        [Required]
        public int ParticipantId { get; set; } // Foreign key to the participant

        [ForeignKey(nameof(ParticipantId))] // Navigation property to the participant
        public Participant Participant { get; set; } = default!; // Navigation property to the participant

        [Required]
        public ParticipantRole Role { get; set; } // Role of the participant in the case

        public int? AddedByUserId { get; set; } // Foreign key to the user who added the participant

        [ForeignKey(nameof(AddedByUserId))] // Navigation property to the user who added the participant
        public User? AddedByUser { get; set; } // Navigation property to the user who added the participant

        [Required]
        public DateTime AddedAt { get; set; } = DateTime.UtcNow; // Timestamp of when the participant was added to the case
    }
}
