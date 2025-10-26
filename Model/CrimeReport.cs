using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BERihalCodestackerChallenge2025.Model
{
    [Index(nameof(Status))] // Index on Status for faster queries
    [Index(nameof(TrackingCode), IsUnique = true)] // Ensure tracking codes are unique
    public class CrimeReport
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(160)]
        public string Title { get; set; } = default!;

        [Required, MaxLength(4000)]
        public string Description { get; set; } = default!;

        [Required, MaxLength(120)]
        public string AreaCity { get; set; } = default!;

        [Required]
        public DateTime ReportDateTime { get; set; } = DateTime.UtcNow;

        [Required]
        public ReportStatus Status { get; set; } = ReportStatus.pending;

        // Citizen = NULL
        public int? ReportedByUserId { get; set; }
        [ForeignKey(nameof(ReportedByUserId))]
        public User? ReportedByUser { get; set; }

        [Precision(9, 6)]
        public decimal? Latitude { get; set; }

        [Precision(9, 6)]
        public decimal? Longitude { get; set; }

        // لتتبع المواطن
        [Required, MaxLength(32)]
        [RegularExpression(@"^[A-Z]{2,4}-\d{4}-\d{3,6}$")] // مثال CR-2025-001234
        public string TrackingCode { get; set; } = default!;

        public ICollection<CaseReport>? CaseLinks { get; set; }
    }
}
