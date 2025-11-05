using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BERihalCodestackerChallenge2025.Model
{
    [Table("Participants")]
    public class Participant
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } // Primary Key

        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty; // Participant full name

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty; // Participant email

        [StringLength(20)]
        public string Phone { get; set; } = string.Empty; // Participant phone number

        [Required, StringLength(50)]
        public string Role { get; set; } = string.Empty; // Participant role (e.g., Witness, Suspect, Victim, etc.)

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Creation timestamp
        public DateTime? UpdatedAt { get; set; } // Last updated timestamp
    }
}
