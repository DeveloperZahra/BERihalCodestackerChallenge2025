using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BERihalCodestackerChallenge2025.Model
{
    [Index(nameof(Status))] // Index on Status for faster queries
    [Index(nameof(TrackingCode), IsUnique = true)] // Ensure tracking codes are unique
    public class CrimeReport // Represents a crime report submitted by a citizen
    {
        [Key]
        public int Id { get; set; } // Primary key

        [Required, MaxLength(160)] // Title of the crime report
        public string Title { get; set; } = default!; // Short title of the report

        [Required, MaxLength(4000)] // Detailed description of the crime
        public string Description { get; set; } = default!; // Full description of the report

        [Required, MaxLength(120)] // Location details
        public string AreaCity { get; set; } = default!; // Area or city where the crime occurred

        [Required]
        public DateTime ReportDateTime { get; set; } = DateTime.UtcNow; // Date and time when the report was created

        [Required]
        public ReportStatus Status { get; set; } = ReportStatus.pending; // Current status of the report

        // Citizen = NULL
        public int? ReportedByUserId { get; set; } // Foreign key to the user who reported the crime
        [ForeignKey(nameof(ReportedByUserId))] // Navigation property to the reporting user
        public User? ReportedByUser { get; set; } // The user who reported the crime

        [Precision(9, 6)] 
        public decimal? Latitude { get; set; } // Latitude of the crime location

        [Precision(9, 6)]
        public decimal? Longitude { get; set; } // Longitude of the crime location


        [Required, MaxLength(32)] // Unique tracking code for the report
        [RegularExpression(@"^[A-Z]{2,4}-\d{4}-\d{3,6}$")] // CR-2025-001234
        public string TrackingCode { get; set; } = default!; // Unique tracking code for the report

        public ICollection<CaseReport>? CaseLinks { get; set; } // Links to cases associated with this report
    }
}
