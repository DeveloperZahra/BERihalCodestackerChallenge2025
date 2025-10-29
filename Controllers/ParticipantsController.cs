using AutoMapper;
using BERihalCodestackerChallenge2025.Data;
using BERihalCodestackerChallenge2025.DTOs;
using BERihalCodestackerChallenge2025.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BERihalCodestackerChallenge2025.Controllers
{
    [ApiController]
    [Route("")]
    [Authorize(Roles = "Admin,Investigator,Officer")]
    public class ParticipantsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ParticipantsController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // ================================================================
        // POST: api/participants
        // Description: Add a new participant (suspect/victim/witness)
        // ================================================================
        [HttpPost("CreateParticipant")]
        public async Task<IActionResult> CreateParticipant([FromBody] ParticipantCreateDto dto)
        {
            //  Map DTO → Model
            var participant = _mapper.Map<Participant>(dto);

            _context.Participants.Add(participant);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetParticipantById), new { id = participant.Id }, participant);
        }

        // ================================================================
        // GET: api/participants
        // Description: Retrieve all participants (Admin/Investigator only)
        // ================================================================
        [Authorize(Roles = "Admin,Investigator")]
        [HttpGet("GetAllParticipants")]
        public async Task<ActionResult<IEnumerable<Participant>>> GetAllParticipants()
        {
            var participants = await _context.Participants.AsNoTracking().ToListAsync();
            return Ok(participants);
        }

        // ================================================================
        // GET: api/participants/{id}
        // Description: Retrieve a specific participant
        // ================================================================
        [HttpGet("GetParticipantById /{id:int}")]
        public async Task<ActionResult<Participant>> GetParticipantById(int id)
        {
            var participant = await _context.Participants.FindAsync(id);
            if (participant == null)
                return NotFound("Participant not found.");

            return Ok(participant);
        }

        // ================================================================
        // PUT: api/participants/{id}
        // Description: Update a participant’s data
        // ================================================================
        [HttpPut("UpdateParticipant/{id:int}")]
        public async Task<IActionResult> UpdateParticipant(int id, [FromBody] ParticipantCreateDto dto)
        {
            var participant = await _context.Participants.FindAsync(id);
            if (participant == null)
                return NotFound("Participant not found.");

            _mapper.Map(dto, participant);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ================================================================
        // DELETE: api/participants/{id}
        // Description: Delete a participant (Admin only)
        // ================================================================
        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteParticipant/{id:int}")]
        public async Task<IActionResult> DeleteParticipant(int id)
        {
            var participant = await _context.Participants.FindAsync(id);
            if (participant == null)
                return NotFound("Participant not found.");

            _context.Participants.Remove(participant);
            await _context.SaveChangesAsync();

            return Ok($"Participant ID {id} deleted successfully.");
        }

        // ================================================================
        // POST: api/participants/assign
        // Description: Assign participant to a case with a specific role
        // ================================================================
        [Authorize(Roles = "Admin,Investigator")]
        [HttpPost("AddAssignParticipant")]
        public async Task<IActionResult> AssignParticipantToCase([FromBody] CaseParticipantCreateDto dto)
        {
            //  Validate case and participant existence
            var caseExists = await _context.Cases.AnyAsync(c => c.Id == dto.CaseId);
            var participantExists = await _context.Participants.AnyAsync(p => p.Id == dto.ParticipantId);

            if (!caseExists || !participantExists)
                return BadRequest("Invalid CaseId or ParticipantId.");

            // Define variable outside the if scope
            ParticipantRole roleEnum;

            //  Convert role string to enum
            if (!Enum.TryParse<ParticipantRole>(dto.Role, true, out roleEnum))
                return BadRequest("Invalid role. Allowed values: suspect, victim, witness.");

            //  Prevent duplicate role assignment
            bool alreadyAssigned = await _context.CaseParticipants.AnyAsync(cp =>
                cp.CaseId == dto.CaseId &&
                cp.ParticipantId == dto.ParticipantId &&
                cp.Role == roleEnum);

            if (alreadyAssigned)
                return Conflict("This participant is already assigned to this case with the same role.");

            //  Map DTO → Entity
            var caseParticipant = _mapper.Map<CaseParticipant>(dto);
            caseParticipant.Role = roleEnum; //  assign parsed enum
            caseParticipant.AddedAt = DateTime.UtcNow;

            //  Identify the user who added this participant
            var addedBy = User?.Identity?.Name;
            var addedUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == addedBy);
            if (addedUser != null)
                caseParticipant.AddedByUserId = addedUser.Id;

            _context.CaseParticipants.Add(caseParticipant);
            await _context.SaveChangesAsync();

            return Ok("Participant assigned successfully.");
        }


        // ================================================================
        // GET: api/participants/case/{caseId}
        // Description: Get all participants in a specific case
        // ================================================================
        [HttpGet("GetParticipantsByCase/{caseId:int}")]
        public async Task<ActionResult<IEnumerable<CaseParticipantReadDto>>> GetParticipantsByCase(int caseId)
        {
            var caseParticipants = await _context.CaseParticipants
                .Include(cp => cp.Participant)
                .Include(cp => cp.AddedByUser)
                .Where(cp => cp.CaseId == caseId)
                .ToListAsync();

            if (!caseParticipants.Any())
                return NotFound("No participants found for this case.");

            return Ok(_mapper.Map<IEnumerable<CaseParticipantReadDto>>(caseParticipants));
        }
    }
}
