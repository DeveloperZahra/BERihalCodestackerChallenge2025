using BERihalCodestackerChallenge2025.Model;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace BERihalCodestackerChallenge2025.Data
{
    // DbContext 
    // ---------------------------
    public class AppDbContext : DbContext // Represents the application's database context
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { } // Constructor accepting DbContext options

        public DbSet<User> Users => Set<User>(); // DbSet for users
        public DbSet<CrimeReport> CrimeReports => Set<CrimeReport>(); // DbSet for crime reports
        public DbSet<Case> Cases => Set<Case>(); // DbSet for cases
        public DbSet<CaseReport> CaseReports => Set<CaseReport>(); // DbSet for case-report links
        public DbSet<CaseAssignee> CaseAssignees => Set<CaseAssignee>();
        public DbSet<Participant> Participants => Set<Participant>();
        public DbSet<CaseParticipant> CaseParticipants => Set<CaseParticipant>();
        public DbSet<Evidence> Evidences => Set<Evidence>();
        public DbSet<EvidenceAuditLog> EvidenceAuditLogs => Set<EvidenceAuditLog>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            // CHECK: Evidence: enforce text vs image payload
            b.Entity<Evidence>().ToTable(tb =>
                tb.HasCheckConstraint("CK_Evidence_Content",
                    "( [Type] = 0 AND [TextContent] IS NOT NULL AND [FileUrl] IS NULL ) OR " +
                    "( [Type] = 1 AND [FileUrl] IS NOT NULL AND [TextContent] IS NULL )"));

            


            b.Entity<Case>()
                .HasOne(c => c.CreatedByUser)
                .WithMany(u => u.CreatedCases)
                .HasForeignKey(c => c.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict); 

            b.Entity<CaseAssignee>()
                .HasOne(a => a.Case).WithMany(c => c.Assignees)
                .HasForeignKey(a => a.CaseId).OnDelete(DeleteBehavior.Cascade);

            b.Entity<CaseReport>()
                .HasOne(cr => cr.Case).WithMany(c => c.LinkedReports)
                .HasForeignKey(cr => cr.CaseId).OnDelete(DeleteBehavior.Cascade);

            b.Entity<CaseParticipant>()
                .HasOne(cp => cp.Case).WithMany(c => c.Participants)
                .HasForeignKey(cp => cp.CaseId).OnDelete(DeleteBehavior.Cascade);

            b.Entity<Evidence>()
                .HasOne(e => e.Case).WithMany(c => c.Evidences)
                .HasForeignKey(e => e.CaseId).OnDelete(DeleteBehavior.Cascade);

            b.Entity<EvidenceAuditLog>()
                .HasOne(a => a.Evidence).WithMany(e => e.Audit)
                .HasForeignKey(a => a.EvidenceId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}


