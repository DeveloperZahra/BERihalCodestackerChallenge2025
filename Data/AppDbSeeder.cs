using System;
using System.Linq;
using BERihalCodestackerChallenge2025.Model;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace BERihalCodestackerChallenge2025.Data
{
    public static class AppDbSeeder 
    {
        public static void SeedDatabase(AppDbContext context) // Seed initial data into the database
        {



            // ---------------------
            // Seed Users
            // ---------------------
            if (!context.Users.Any()) // Seed initial users
            {
                var admin = new User // Admin user
                {
                    Username = "admin", // Admin username
                    Email = "admin@crimecase.local", // Admin email
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"), // Hashed password
                    Role = Role.Admin, // Admin role
                    ClearanceLevel = Clearance.critical, // Highest clearance
                    CreatedAt = DateTime.UtcNow // Timestamp
                };

                var investigator = new User // Investigator user
                {
                    Username = "investigator1", // Investigator username
                    Email = "investigator@crimecase.local", // Investigator email
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Inv@123"), // Hashed password
                    Role = Role.Investigator, // Investigator role
                    ClearanceLevel = Clearance.high, // High clearance
                    CreatedAt = DateTime.UtcNow // Timestamp
                };

                var officer = new User // Officer user
                {
                    Username = "officer1", // Officer username
                    Email = "officer@crimecase.local", // Officer email
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Off@123"), // Hashed password
                    Role = Role.Officer, // Officer role
                    ClearanceLevel = Clearance.medium, // Medium clearance
                    CreatedAt = DateTime.UtcNow // Timestamp
                };

                context.Users.AddRange(admin, investigator, officer); // Add users to context
                context.SaveChanges(); // Save changes to database
            }

            // ---------------------
            // Seed Crime Reports
            // ---------------------
            if (!context.CrimeReports.Any()) // Seed initial crime reports 
            {
                var report = new CrimeReport // Crime report
                {
                    Title = "Robbery at City Center", // Report title
                    Description = "A suspected robbery was reported near downtown at 10 PM.", // Report description
                    AreaCity = "Muscat", // Area/City
                    ReportDateTime = DateTime.UtcNow.AddDays(-1), // Reported date/time
                    Status = ReportStatus.pending, // Report status
                    Latitude = 23.5880M, // Report latitude
                    Longitude = 58.3829M, // Report longitude
                    TrackingCode = "CR-2025-0001" // Unique tracking code
                };

                context.CrimeReports.Add(report);  // Add report to context
                context.SaveChanges(); 
            }

            // ---------------------
            // Seed Cases
            // ---------------------
            if (!context.Cases.Any()) // Seed initial cases
            {
                var adminUser = context.Users.First(u => u.Role == Role.Admin); // Get admin user
                var report = context.CrimeReports.First(); // Get the crime report

                var case1 = new Case // Case
                {
                    CaseNumber = "CASE-2025-001", // Unique case number
                    Name = "City Center Robbery",
                    Description = "Investigation into a robbery case reported by citizen near Muscat City Center.", // Case description
                    AreaCity = "Muscat", // Area/City
                    CaseType = "Robbery", // Case type
                    AuthorizationLevel = Clearance.high, // Required clearance level
                    Status = CaseStatus.ongoing, // Case status
                    CreatedByUserId = adminUser.Id, // Creator user ID
                    CreatedAt = DateTime.UtcNow // Timestamp
                };

                context.Cases.Add(case1); // Add case to context
                context.SaveChanges(); // Save changes to database

                // 
                var link = new CaseReport // Link case to report
                {
                    CaseId = case1.Id, // Case ID
                    ReportId = report.Id, // Report ID
                    LinkedAt = DateTime.UtcNow // Timestamp
                };
                context.CaseReports.Add(link); // Add link to context
                context.SaveChanges();

                // ---------------------
                // Seed Case Assignees
                // ---------------------
                var investigator = context.Users.First(u => u.Role == Role.Investigator); // Get investigator user
                var officer = context.Users.First(u => u.Role == Role.Officer); // Get officer user

                var assign1 = new CaseAssignee // Case assignee
                {
                    CaseId = case1.Id, // Case ID
                    UserId = investigator.Id, // User ID
                    AssignedRole = "Investigator", // Assigned role
                    ProgressStatus = CaseStatus.ongoing, // Progress status
                    AssignedAt = DateTime.UtcNow // Timestamp
                };

                var assign2 = new CaseAssignee // Case assignee
                {
                    CaseId = case1.Id, //   Case ID
                    UserId = officer.Id, // User ID
                    AssignedRole = "Officer", // Assigned role
                    ProgressStatus = CaseStatus.pending, // Progress status
                    AssignedAt = DateTime.UtcNow // Timestamp
                };

                context.CaseAssignees.AddRange(assign1, assign2); // Add assignees to context
                context.SaveChanges();

                // ---------------------
                // Seed Case Participants
                // ---------------------
                var suspect = new Participant
                {
                    FullName = "John Doe", // Suspect name
                    Phone = "987654321", // Suspect phone
                    Notes = "Main suspect seen near the crime scene." // Notes
                };

                var victim = new Participant // Victim participant
                {
                    FullName = "Jane Smith", // Victim name
                    Phone = "912345678", // Victim phone
                    Notes = "Victim reported missing wallet and phone." // Notes
                };

                context.Participants.AddRange(suspect, victim); // Add participants to context
                context.SaveChanges();

                var caseSuspect = new CaseParticipant // Case participant - suspect
                {
                    CaseId = case1.Id, // Link to case
                    ParticipantId = suspect.Id, // Link to suspect participant
                    Role = ParticipantRole.suspect,
                    AddedByUserId = investigator.Id, // Added by investigator
                    AddedAt = DateTime.UtcNow // Timestamp
                };

                var caseVictim = new CaseParticipant // Case participant - victim
                {
                    CaseId = case1.Id,
                    ParticipantId = victim.Id, //   Link to victim participant
                    Role = ParticipantRole.victim, // Victim role
                    AddedByUserId = officer.Id, // Added by officer
                    AddedAt = DateTime.UtcNow // Timestamp
                };

                context.CaseParticipants.AddRange(caseSuspect, caseVictim);
                context.SaveChanges();

                // ---------------------
                // Seed Evidence
                // ---------------------
                var evidence = new Evidence
                {
                    CaseId = case1.Id, // Link to case
                    AddedByUserId = investigator.Id,
                    Type = EvidenceType.text, // Evidence type
                    TextContent = "Fingerprint found on victim's car.", // Text evidence
                    IsSoftDeleted = false, // Not deleted
                    CreatedAt = DateTime.UtcNow,// Timestamp
                    UpdatedAt = DateTime.UtcNow
                };
                context.Evidences.Add(evidence); // Add evidence to context
                context.SaveChanges();

                // ---------------------
                // Seed Evidence Audit Log
                // ---------------------
                var audit = new EvidenceAuditLog // Evidence audit log
                {
                    EvidenceId = evidence.Id,
                    Action = "add",
                    ActedByUserId = investigator.Id,
                    ActedAt = DateTime.UtcNow,
                    Details = "Initial evidence entry by Investigator." // Details
                };

                context.EvidenceAuditLogs.Add(audit); // Add audit log to context
                context.SaveChanges();
            }
        }
    }
}

