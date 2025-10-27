using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BERihalCodestackerChallenge2025.Model
{
   
    // Participants (people, not system users)
    
    [Index(nameof(FullName))] // Index on FullName for faster searches
    public class Participant
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(160)]
        public string FullName { get; set; } = default!;

        [MaxLength(32), Phone]
        public string? Phone { get; set; }

        [MaxLength(1024)]
        public string? Notes { get; set; }

        public ICollection<CaseParticipant>? CaseLinks { get; set; }
    }
}
