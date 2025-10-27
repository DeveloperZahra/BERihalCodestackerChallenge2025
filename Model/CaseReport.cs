using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations; 

namespace BERihalCodestackerChallenge2025.Model 
{
    [Index(nameof(CaseId), nameof(ReportId), IsUnique = true)] // Ensure a report is linked only once per case
    public class CaseReport 
    {
        [Key] 
        public int Id { get; set; } // Primary key

        [Required]
        public int CaseId { get; set; } // Foreign key to the associated case

        [ForeignKey(nameof(CaseId))] 
        public Case Case { get; set; } = default!; // Navigation property to the associated case

        [Required]
        public int ReportId { get; set; } 

        [ForeignKey(nameof(ReportId))] 
        public CrimeReport Report { get; set; } = default!; 

        [Required]
        public DateTime LinkedAt { get; set; } = DateTime.UtcNow; 
    }
}
