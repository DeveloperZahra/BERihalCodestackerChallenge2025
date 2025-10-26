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

        [Required, MaxLength(64)]
        public string Username { get; set; } = default!;

        [Required, MaxLength(128), EmailAddress]
        public string Email { get; set; } = default!;

        [Required] //  PBKDF2/BCrypt
        public string PasswordHash { get; set; } = default!;

        [Required]
        public Role Role { get; set; }

        [Required]
        public Clearance ClearanceLevel { get; set; } = Clearance.low;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

         
        public ICollection<Case>? CreatedCases { get; set; }
        public ICollection<CaseAssignee>? CaseAssignments { get; set; }
    }

}
