using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace BERihalCodestackerChallenge2025.Model
{
    [Index(nameof(Username), IsUnique = true)] // Ensure usernames are unique
    [Index(nameof(Email), IsUnique = true)] // Ensure emails are unique
    public class User // Represents a user in the system
    {
        [Key]
        public int Id { get; set; } // Primary key

        [Required, MaxLength(64)] // Username must be unique
        public string Username { get; set; } = default!; // User's unique username

        [Required, MaxLength(128), EmailAddress] // Email must be unique and valid format
        public string Email { get; set; } = default!; // User's unique email address

        [Required] //  PBKDF2/BCrypt
        public string PasswordHash { get; set; } = default!;// Hashed password for security

        [Required]
        public Role Role { get; set; } // User's role in the system

        [Required]
        public Clearance ClearanceLevel { get; set; } = Clearance.low;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

         
        public ICollection<Case>? CreatedCases { get; set; }
        public ICollection<CaseAssignee>? CaseAssignments { get; set; }
    }

}
