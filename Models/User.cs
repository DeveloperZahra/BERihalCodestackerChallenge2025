using System.ComponentModel.DataAnnotations;

namespace BERihalCodestackerChallenge2025.Models
{
    // Represents system users (Admin, Investigator, Officer)
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(50)]
        public string Username { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required, StringLength(150)]
        public string FullName { get; set; }

        [Required]
        public string Role { get; set; } // Admin | Investigator | Officer

        [Required]
        public string ClearanceLevel { get; set; } // low | medium | high | critical

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
