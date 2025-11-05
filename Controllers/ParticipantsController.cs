using BERihalCodestackerChallenge2025.DTOs;
using BERihalCodestackerChallenge2025.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BERihalCodestackerChallenge2025.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Investigator,Officer")]
    public class ParticipantsController : ControllerBase
    {
        private readonly IParticipantService _participantService;

        public ParticipantsController(IParticipantService participantService)
        {
            _participantService = participantService;
        }

        // ================================================================
        // POST: api/participants/CreateParticipant
        // ================================================================
        [HttpPost("CreateParticipant")]
        public async Task<IActionResult> CreateParticipant([FromBody] ParticipantCreateUpdateDto dto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _participantService.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetParticipantById), new { id = created.Id }, created);
        }

        // ================================================================
        // GET: api/participants/GetAllParticipants
        // ================================================================
        [Authorize(Roles = "Admin,Investigator")]
        [HttpGet("GetAllParticipants")]
        public async Task<IActionResult> GetAllParticipants(CancellationToken ct)
        {
            var participants = await _participantService.GetAllAsync(ct);
            return Ok(participants);
        }

        // ================================================================
        // GET: api/participants/GetParticipantById/{id}
        // ================================================================
        [HttpGet("GetParticipantById/{id:int}")]
        public async Task<IActionResult> GetParticipantById(int id, CancellationToken ct)
        {
            var participant = await _participantService.GetByIdAsync(id, ct);
            if (participant is null)
                return NotFound("Participant not found.");

            return Ok(participant);
        }

        // ================================================================
        // PUT: api/participants/UpdateParticipant/{id}
        // ================================================================
        [HttpPut("UpdateParticipant/{id:int}")]
        public async Task<IActionResult> UpdateParticipant(int id, [FromBody] ParticipantCreateUpdateDto dto, CancellationToken ct)
        {
            try
            {
                await _participantService.UpdateAsync(id, dto, ct);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Participant not found.");
            }
        }

        // ================================================================
        // DELETE: api/participants/DeleteParticipant/{id}
        // ================================================================
        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteParticipant/{id:int}")]
        public async Task<IActionResult> DeleteParticipant(int id, CancellationToken ct)
        {
            try
            {
                await _participantService.DeleteAsync(id, ct);
                return Ok($"Participant ID {id} deleted successfully.");
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Participant not found.");
            }
        }
    }
}
