using BERihalCodestackerChallenge2025.DTOs;
using BERihalCodestackerChallenge2025.Model;
using BERihalCodestackerChallenge2025.Repositories;
using BERihalCodestackerChallenge2025.Services;
using Microsoft.EntityFrameworkCore;

namespace BERihalCodestackerChallenge2025.Services
{
    public class CaseService : ICaseService
    {
        private readonly IUnitOfWork _uow; // Unit of Work for managing repositories and transactions
        public CaseService(IUnitOfWork uow) => _uow = uow; // Constructor accepting the Unit of Work

        public async Task<(int caseId, string caseNumber)> CreateAsync(int createdByUserId, CaseCreateDto dto, CancellationToken ct = default) // Create a new case and return its ID and case number
        {
            var level = Enum.Parse<Clearance>(dto.AuthorizationLevel, true); // Parse the authorization level from the DTO

            var entity = new Case
            {
                CaseNumber = $"CASE-{DateTime.UtcNow:yyyy}-{Random.Shared.Next(1, 999999):000000}", // Generate a unique case number
                Name = dto.Name, // Set the case name from the DTO
                Description = dto.Description, // Set the case description from the DTO
                AreaCity = dto.AreaCity, // Set the area/city from the DTO
                CaseType = dto.CaseType, // Set the case type from the DTO
                AuthorizationLevel = level, // Set the authorization level
                Status = CaseStatus.pending, // Set the initial status to pending
                CreatedByUserId = createdByUserId, // Set the ID of the user who created the case
                CreatedAt = DateTime.UtcNow
            };

            await _uow.Cases.AddAsync(entity, ct); // Add the new case entity to the repository

            if (dto.ReportIds is not null && dto.ReportIds.Count > 0) 
                await _uow.Cases.LinkReportsAsync(entity.CaseId, dto.ReportIds, ct); // Link any associated reports to the case

            await _uow.SaveChangesAsync(ct); // Save changes to the database
            return (entity.CaseId, entity.CaseNumber); // Return the new case ID and case number
        }

        public async Task UpdateAsync(int caseId, CaseUpdateDto dto, CancellationToken ct = default) // Update an existing case with new details
        {
            var c = await _uow.Cases.GetByIdAsync(caseId, ct) ?? throw new KeyNotFoundException("Case not found"); // Retrieve the case by ID or throw an exception if not found
            if (!string.IsNullOrWhiteSpace(dto.Name)) c.Name = dto.Name; // Update the case name if provided
            if (!string.IsNullOrWhiteSpace(dto.Description)) c.Description = dto.Description; // Update the case description if provided
            if (!string.IsNullOrWhiteSpace(dto.AreaCity)) c.AreaCity = dto.AreaCity; // Update the area/city if provided
            if (!string.IsNullOrWhiteSpace(dto.CaseType)) c.CaseType = dto.CaseType; // Update the case type if provided
            if (!string.IsNullOrWhiteSpace(dto.AuthorizationLevel)) 
                c.AuthorizationLevel = Enum.Parse<Clearance>(dto.AuthorizationLevel, true); // Update the authorization level if provided

            _uow.Cases.Update(c); // Mark the case entity as updated
            await _uow.SaveChangesAsync(ct); // Save changes to the database
        }

        public async Task SetStatusAsync(int caseId, CaseStatus status, CancellationToken ct = default) // Set the status of a case
        {
            var c = await _uow.Cases.GetByIdAsync(caseId, ct) ?? throw new KeyNotFoundException("Case not found"); // Retrieve the case by ID or throw an exception if not found
            c.Status = status; // Update the case status
            _uow.Cases.Update(c); // Mark the case entity as updated
            await _uow.SaveChangesAsync(ct); // Save changes to the database
        }

        public async Task AssignOfficerAsync(int caseId, int officerUserId, CancellationToken ct = default) // Assign an officer to a case
        {
            var c = await _uow.Cases.GetByIdAsync(caseId, ct) ?? throw new KeyNotFoundException("Case not found"); // Retrieve the case by ID or throw an exception if not found
            var officer = await _uow.Users.GetByIdAsync(officerUserId, ct) ?? throw new KeyNotFoundException("Officer not found"); // Retrieve the officer user by ID or throw an exception if not found
            if (officer.Role != Role.Officer) throw new InvalidOperationException("Not an officer."); // Ensure the user is an officer
            if (officer.ClearanceLevel < c.AuthorizationLevel) throw new InvalidOperationException("Officer clearance is lower than case authorization."); // Ensure the officer has sufficient clearance

            var exists = await _uow.CaseAssignees.ExistsAsync(caseId, officerUserId, ct); // Check if the officer is already assigned to the case
            if (!exists) // If not already assigned, create a new assignment
            {
                await _uow.CaseAssignees.AddAsync(new CaseAssignee // Create a new case assignee entity
                {
                    CaseId = caseId, // Set the case ID
                    UserId = officerUserId, // Set the officer user ID
                    AssignedRole = "Officer", // Set the assigned role
                    ProgressStatus = CaseStatus.pending, // Set the initial progress status to pending
                    AssignedAt = DateTime.UtcNow // Set the current timestamp for when the officer is assigned
                }, ct); // Add the new case assignee entity to the repository
                await _uow.SaveChangesAsync(ct); // Save changes to the database
            }
        }

        public async Task<IEnumerable<CaseListItemDto>> ListAsync(string? q, CancellationToken ct = default) // List cases with optional search query
        {
            var list = await _uow.Cases.SearchAsync(q, ct); // Search for cases based on the query string
            return list.Select(c => new CaseListItemDto // Map each case entity to a CaseListItemDto
            {
                CaseNumber = c.CaseNumber, // Set the case number
                Name = c.Name, // Set the case name
                Description = TextUtils.TrimWords(c.Description, 100), // Trim the description to 100 words
                AreaCity = c.AreaCity, // Set the area/city
                CreatedBy = c.CreatedByUser.Username, // Set the username of the user who created the case
                CreatedAt = c.CreatedAt, // Set the creation timestamp
                CaseType = c.CaseType, // Set the case type
                AuthorizationLevel = c.AuthorizationLevel.ToString() // Set the authorization level as a string
            });
        }

        public async Task<object?> GetDetailsAsync(int id, CancellationToken ct = default) // Get detailed information about a specific case by its ID
        {
            var c = await _uow.Cases.GetDetailsAsync(id, ct); // Retrieve the case details by ID
            if (c is null) return null; // Return null if the case is not found

            var reportedBy = c.LinkedReports.Select(l => l.Report.ReportedByUserId == null ? "Citizen" :
                                 l.Report.ReportedByUser!.Username).FirstOrDefault(); // Get the username of the user who reported the case, or "Citizen" if reported by a citizen

            return new // Return an anonymous object with detailed case information
            {
                c.CaseNumber,  // Unique case number
                CaseName = c.Name, // Name of the case
                c.Description, // Full description of the case
                c.AreaCity, // Area or city of the case
                CreatedBy = c.CreatedByUser.Username, // Username of the user who created the case
                c.CreatedAt, // Creation timestamp
                c.CaseType, // Case type of the case
                AuthorizationLevel = c.AuthorizationLevel.ToString(), // Convert authorization level to string
                ReportedBy = reportedBy, // Username of the reporter or "Citizen"
                AssigneesCount = c.Assignees.Count, 
                EvidencesCount = c.Evidences.Count(e => !e.IsSoftDeleted), // Count of non-soft-deleted evidences linked to the case
                SuspectsCount = c.Participants.Count(p => p.Role == ParticipantRole.suspect), // Count of suspects involved in the case
                VictimsCount = c.Participants.Count(p => p.Role == ParticipantRole.victim), // Count of victims involved in the case
                WitnessesCount = c.Participants.Count(p => p.Role == ParticipantRole.witness) // Count of witnesses involved in the case
            };
        }
    }
}

