using System;
using System.Linq;
using BERihalCodestackerChallenge2025.Model;
using Microsoft.EntityFrameworkCore;

namespace BERihalCodestackerChallenge2025.Data
{
    public static class AppDbSeeder 
    {
        public static void SeedDatabase(AppDbContext context) 
        {



            // ---------------------
            // Seed Users
            // ---------------------
            if (!context.Users.Any())
            {
                var admin = new User
                {
                    Username = "admin",
                    Email = "admin@crimecase.local", 
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"), //
                    Role = Role.Admin,
                    ClearanceLevel = Clearance.critical,
                    CreatedAt = DateTime.UtcNow
                };

                var investigator = new User
                {
                    Username = "investigator1",
                    Email = "investigator@crimecase.local",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Inv@123"),
                    Role = Role.Investigator,
                    ClearanceLevel = Clearance.high,
                    CreatedAt = DateTime.UtcNow
                };

                var officer = new User
                {
                    Username = "officer1",
                    Email = "officer@crimecase.local",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Off@123"),
                    Role = Role.Officer,
                    ClearanceLevel = Clearance.medium,
                    CreatedAt = DateTime.UtcNow
                };

                context.Users.AddRange(admin, investigator, officer);
                context.SaveChanges();
            }

            // ---------------------
            // Seed Crime Reports
            // ---------------------
            if (!context.CrimeReports.Any())
            {
                var report = new CrimeReport
                {
                    Title = "Robbery at City Center",
                    Description = "A suspected robbery was reported near downtown at 10 PM.",
                    AreaCity = "Muscat",
                    ReportDateTime = DateTime.UtcNow.AddDays(-1),
                    Status = ReportStatus.pending,
                    Latitude = 23.5880M,
                    Longitude = 58.3829M,
                    TrackingCode = "CR-2025-0001"
                };

                context.CrimeReports.Add(report);
                context.SaveChanges();
            }

            // ---------------------
            // Seed Cases
            // ---------------------
            if (!context.Cases.Any())
            {
                var adminUser = context.Users.First(u => u.Role == Role.Admin);
                var report = context.CrimeReports.First();

                var case1 = new Case
                {
                    CaseNumber = "CASE-2025-001",
                    Name = "City Center Robbery",
                    Description = "Investigation into a robbery case reported by citizen near Muscat City Center.",
                    AreaCity = "Muscat",
                    CaseType = "Robbery",
                    AuthorizationLevel = Clearance.high,
                    Status = CaseStatus.ongoing,
                    CreatedByUserId = adminUser.Id,
                    CreatedAt = DateTime.UtcNow
                };

                context.Cases.Add(case1);
                context.SaveChanges();

                // 
                var link = new CaseReport
                {
                    CaseId = case1.Id,
                    ReportId = report.Id,
                    LinkedAt = DateTime.UtcNow
                };
                context.CaseReports.Add(link);
                context.SaveChanges();

                // ---------------------
                
                // ---------------------
                var investigator = context.Users.First(u => u.Role == Role.Investigator);
                var officer = context.Users.First(u => u.Role == Role.Officer);

                var assign1 = new CaseAssignee
                {
                    CaseId = case1.Id,
                    UserId = investigator.Id,
                    AssignedRole = "Investigator",
                    ProgressStatus = CaseStatus.ongoing,
                    AssignedAt = DateTime.UtcNow
                };

                var assign2 = new CaseAssignee
                {
                    CaseId = case1.Id,
                    UserId = officer.Id,
                    AssignedRole = "Officer",
                    ProgressStatus = CaseStatus.pending,
                    AssignedAt = DateTime.UtcNow
                };

                context.CaseAssignees.AddRange(assign1, assign2);
                context.SaveChanges();

                // ---------------------
                
                // ---------------------
                var suspect = new Participant
                {
                    FullName = "John Doe",
                    Phone = "987654321",
                    Notes = "Main suspect seen near the crime scene."
                };

                var victim = new Participant
                {
                    FullName = "Jane Smith",
                    Phone = "912345678",
                    Notes = "Victim reported missing wallet and phone."
                };

                context.Participants.AddRange(suspect, victim);
                context.SaveChanges();

                var caseSuspect = new CaseParticipant
                {
                    CaseId = case1.Id,
                    ParticipantId = suspect.Id,
                    Role = ParticipantRole.suspect,
                    AddedByUserId = investigator.Id,
                    AddedAt = DateTime.UtcNow
                };

                var caseVictim = new CaseParticipant
                {
                    CaseId = case1.Id,
                    ParticipantId = victim.Id,
                    Role = ParticipantRole.victim,
                    AddedByUserId = officer.Id,
                    AddedAt = DateTime.UtcNow
                };

                context.CaseParticipants.AddRange(caseSuspect, caseVictim);
                context.SaveChanges();

                // ---------------------
                
                // ---------------------
                var evidence = new Evidence
                {
                    CaseId = case1.Id,
                    AddedByUserId = investigator.Id,
                    Type = EvidenceType.text,
                    TextContent = "Fingerprint found on victim's car.",
                    IsSoftDeleted = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                context.Evidences.Add(evidence);
                context.SaveChanges();

                // ---------------------
                
                // ---------------------
                var audit = new EvidenceAuditLog
                {
                    EvidenceId = evidence.Id,
                    Action = "add",
                    ActedByUserId = investigator.Id,
                    ActedAt = DateTime.UtcNow,
                    Details = "Initial evidence entry by Investigator."
                };

                context.EvidenceAuditLogs.Add(audit);
                context.SaveChanges();
            }
        }
    }
}

