using BERihalCodestackerChallenge2025.Model;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Emit;



namespace BERihalCodestackerChallenge2025.Data
{
    // DbContext 
    // ---------------------------
    public class AppDbContext : DbContext // Represents the application's database context
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)  : base(options) { } // Constructor accepting DbContext options

        public DbSet<User> Users { get; set; } // DbSet for users
        public DbSet<CrimeReport> CrimeReports { get; set; } // DbSet for crime reports
        public DbSet<Case> Cases { get; set; } // DbSet for cases
        public DbSet<CaseReport> CaseReports { get; set; } // DbSet for case-report links
        public DbSet<CaseAssignee> CaseAssignees { get; set; } // DbSet for case assignees
        public DbSet<Participant> Participants { get; set; } // DbSet for participants
        public DbSet<CaseParticipant> CaseParticipants { get; set; } // DbSet for case-participant links
        public DbSet<Evidence> Evidences { get; set; } // DbSet for evidences
        public DbSet<EvidenceAuditLog> EvidenceAuditLogs { get; set; } // DbSet for evidence audit logs



        protected override void OnModelCreating(ModelBuilder b) // Configure entity relationships and constraints
        {
            base.OnModelCreating(b); // Call base method

            // CHECK: Evidence: enforce text vs image payload
            b.Entity<Evidence>().ToTable(tb =>
                tb.HasCheckConstraint("CK_Evidence_Content",
                    "( [Type] = 0 AND [TextContent] IS NOT NULL AND [FileUrl] IS NULL ) OR " +
                    "( [Type] = 1 AND [FileUrl] IS NOT NULL AND [TextContent] IS NULL )")); // Text vs File constraint




            b.Entity<Case>() 
                .HasOne(c => c.CreatedByUser)
                .WithMany(u => u.CreatedCases)
                .HasForeignKey(c => c.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);  // Prevent deletion of users who created cases

            b.Entity<CaseAssignee>()
                .HasOne(ca => ca.Case)
                .WithMany(c => c.CaseAssignees)
                .HasForeignKey(ca => ca.CaseId)
                .OnDelete(DeleteBehavior.Restrict); // Cascade delete assignees when case is deleted

            b.Entity<CaseAssignee>()
                .HasOne(ca => ca.User)
                .WithMany(u => u.CaseAssignees)
                .HasForeignKey(ca => ca.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            b.Entity<CaseReport>()
                .HasOne(cr => cr.Case).WithMany(c => c.LinkedReports)
                .HasForeignKey(cr => cr.CaseId).OnDelete(DeleteBehavior.Cascade); // Cascade delete case-report links when case is deleted

            b.Entity<CaseParticipant>()
                .HasOne(cp => cp.Case).WithMany(c => c.Participants)
                .HasForeignKey(cp => cp.CaseId).OnDelete(DeleteBehavior.Cascade); // Cascade delete case-participant links when case is deleted

            b.Entity<Evidence>()
                .HasOne(e => e.Case).WithMany(c => c.Evidences)
                .HasForeignKey(e => e.CaseId).OnDelete(DeleteBehavior.NoAction); // Cascade delete evidences when case is deleted

            b.Entity<EvidenceAuditLog>()
                .HasOne(a => a.Evidence)
                .WithMany(e => e.Audit)
                .HasForeignKey(a => a.EvidenceId)
                .OnDelete(DeleteBehavior.NoAction); // Cascade delete audit logs when evidence is deleted

            b.Entity<EvidenceAuditLog>()
                 .Property(p => p.ActedBy)
                 .HasMaxLength(200)
                 .IsRequired(false);

        }
    }
}


