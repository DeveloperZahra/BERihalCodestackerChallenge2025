using BERihalCodestackerChallenge2025.Data;
using BERihalCodestackerChallenge2025.DTOs;
using BERihalCodestackerChallenge2025.Model;
using BERihalCodestackerChallenge2025.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BERihalCodestackerChallenge2025.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Investigator,Officer")]
    public class ParticipantsController : ControllerBase
    {
        private readonly IParticipantService _participants;

        public ParticipantsController(IParticipantService participantService)
        {
            _participants = participantService;
        }

        // ================================================================
        // POST: api/participants/CreateParticipant
        // Description: Create a new participant with error handling
        // ================================================================
        [HttpPost("CreateParticipant")]
        public async Task<IActionResult> CreateParticipant([FromBody] ParticipantCreateUpdateDto dto, CancellationToken ct)
        {
            try
            {
                //  Verifying the validity of data received from the customer
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        message = "Invalid participant data. Please check input fields.",
                        errors = ModelState.Values
                                           .SelectMany(v => v.Errors)
                                           .Select(e => e.ErrorMessage)
                                           .ToList()
                    });
                }

                //  Call the service to create the participant
                var created = await _participants.CreateAsync(dto, ct);

                // Returns success response (201 Created)
                return CreatedAtAction(nameof(GetParticipantById), new { id = created.Id }, new
                {
                    message = "Participant created successfully.",
                    participant = created
                });
            }
            catch (ArgumentNullException ex)
            {
                //  Incomplete values
                return BadRequest(new
                {
                    message = "Missing required participant data.",
                    details = ex.Message
                });
            }
            catch (KeyNotFoundException ex)
            {
                // Participant status: Not found (rarely in Create, but for standardization)
                return NotFound(new
                {
                    message = "Participant not found.",
                    details = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                // Logical error (e.g., attempting to enter a duplicate email)
                return Conflict(new
                {
                    message = "Cannot create participant due to invalid state or duplicate data.",
                    details = ex.Message
                });
            }
            catch (DbUpdateException ex)
            {
                //  Database-level error
                return StatusCode(500, new
                {
                    message = "Database error occurred while saving participant.",
                    details = ex.InnerException?.Message ?? ex.Message
                });
            }
            catch (Exception ex)
            {
                //  Unexpected general error
                return StatusCode(500, new
                {
                    message = "An unexpected error occurred while creating participant.",
                    details = ex.Message
                });
            }
        }

        // ================================================================
        // GET: api/participants/GetAllParticipants
        // Description: Retrieve all participants with error handling
        // ================================================================
        [Authorize(Roles = "Admin,Investigator")]
        [HttpGet("GetAllParticipants")]
        public async Task<IActionResult> GetAllParticipants(CancellationToken ct)
        {
            try
            {
                // Bring all participants from the service
                var participants = await _participants.GetAllAsync(ct);

                //Data verification
                if (participants == null || !participants.Any())
                {
                    return NotFound(new
                    {
                        message = "No participants found in the system."
                    });
                }

                //  Success of the operation
                return Ok(new
                {
                    message = "Participants retrieved successfully.",
                    total = participants.Count(),
                    data = participants
                });
            }
            catch (InvalidOperationException ex)
            {
                // Logical error during reading (rarely but as a precaution)
                return BadRequest(new
                {
                    message = "An invalid operation occurred while retrieving participants.",
                    details = ex.Message
                });
            }
            catch (DbUpdateException ex)
            {
                // Database error
                return StatusCode(500, new
                {
                    message = "Database error occurred while retrieving participants.",
                    details = ex.InnerException?.Message ?? ex.Message
                });
            }
            catch (Exception ex)
            {
                // Unexpected general error
                return StatusCode(500, new
                {
                    message = "An unexpected error occurred while retrieving participants.",
                    details = ex.Message
                });
            }
        }


        // ================================================================
        // GET: api/participants/GetParticipantById/{id}
        // Description: Retrieve a specific participant by ID with error handling
        // ================================================================
        [HttpGet("GetParticipantById")]
        public async Task<IActionResult> GetParticipantById(int id, CancellationToken ct)
        {
            try
            {
                // Validate the input ID
                if (id <= 0)
                {
                    return BadRequest(new
                    {
                        message = "Invalid participant ID. ID must be greater than zero."
                    });
                }

                //  Attempt to retrieve the participant by ID from the service
                var participant = await _participants.GetByIdAsync(id, ct);

                //  If not found, return 404
                if (participant is null)
                {
                    return NotFound(new
                    {
                        message = $"Participant with ID {id} was not found."
                    });
                }

                //  Success response
                return Ok(new
                {
                    message = "Participant retrieved successfully.",
                    data = participant
                });
            }
            catch (KeyNotFoundException ex)
            {
                //  Handles explicit not-found exception thrown from the service
                return NotFound(new
                {
                    message = "Participant not found.",
                    details = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                //  Handles logical or state-related issues (e.g., repository errors)
                return BadRequest(new
                {
                    message = "An invalid operation occurred while retrieving the participant.",
                    details = ex.Message
                });
            }
            catch (DbUpdateException ex)
            {
                //  Handles database-related exceptions
                return StatusCode(500, new
                {
                    message = "A database error occurred while retrieving the participant.",
                    details = ex.InnerException?.Message ?? ex.Message
                });
            }
            catch (Exception ex)
            {
                //  Handles any unexpected exceptions
                return StatusCode(500, new
                {
                    message = "An unexpected error occurred while retrieving the participant.",
                    details = ex.Message
                });
            }
        }


        // ================================================================
        // PUT: api/participants/UpdateParticipant/{id}
        // Description: Update an existing participant by ID with full error handling
        // ================================================================
        [HttpPut("UpdateParticipant")]
        public async Task<IActionResult> UpdateParticipant(int id, [FromBody] ParticipantCreateUpdateDto dto, CancellationToken ct)
        {
            try
            {
                //  Validate the provided ID
                if (id <= 0)
                {
                    return BadRequest(new
                    {
                        message = "Invalid participant ID. ID must be greater than zero."
                    });
                }

                //  Validate the incoming data model
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        message = "Invalid participant data. Please check input fields.",
                        errors = ModelState.Values
                                           .SelectMany(v => v.Errors)
                                           .Select(e => e.ErrorMessage)
                                           .ToList()
                    });
                }

                //  Attempt to update the participant using the service
                await _participants.UpdateAsync(id, dto, ct);

                //  Return 204 NoContent if the update was successful
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                //  The specified participant was not found in the database
                return NotFound(new
                {
                    message = "Participant not found.",
                    details = ex.Message
                });
            }
            catch (ArgumentNullException ex)
            {
                //  Null or missing data in the request
                return BadRequest(new
                {
                    message = "Missing required participant data.",
                    details = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                //  Logical or business rule violation (e.g., duplicate email)
                return Conflict(new
                {
                    message = "Cannot update participant due to invalid state or duplicate data.",
                    details = ex.Message
                });
            }
            catch (DbUpdateException ex)
            {
                //  Database-level error (e.g., foreign key or constraint issue)
                return StatusCode(500, new
                {
                    message = "A database error occurred while updating the participant.",
                    details = ex.InnerException?.Message ?? ex.Message
                });
            }
            catch (Exception ex)
            {
                // Catch any unexpected errors and prevent API crash
                return StatusCode(500, new
                {
                    message = "An unexpected error occurred while updating the participant.",
                    details = ex.Message
                });
            }
        }


        // ================================================================
        // DELETE: api/participants/DeleteParticipant/{id}
        // Description: Permanently delete a participant by ID with full error handling
        // ================================================================
        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteParticipant")]
        public async Task<IActionResult> DeleteParticipant(int id, CancellationToken ct)
        {
            try
            {
                //  Validate the provided ID
                if (id <= 0)
                {
                    return BadRequest(new
                    {
                        message = "Invalid participant ID. ID must be greater than zero."
                    });
                }

                // Attempt to delete the participant using the service
                await _participants.DeleteAsync(id, ct);

                //  Return success message if deletion is successful
                return Ok(new
                {
                    message = $"Participant with ID {id} deleted successfully."
                });
            }
            catch (KeyNotFoundException ex)
            {
                //  The participant with the given ID was not found in the database
                return NotFound(new
                {
                    message = "Participant not found.",
                    details = ex.Message
                });
            }
            catch (DbUpdateException ex)
            {
                //  Database-level error (e.g., foreign key constraint violation)
                return StatusCode(500, new
                {
                    message = "A database error occurred while deleting the participant.",
                    details = ex.InnerException?.Message ?? ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                //  Logical or state-related issue preventing deletion
                return Conflict(new
                {
                    message = "Cannot delete participant due to a logical conflict or dependency.",
                    details = ex.Message
                });
            }
            catch (Exception ex)
            {
                //  Catch any unexpected errors to prevent API crash
                return StatusCode(500, new
                {
                    message = "An unexpected error occurred while deleting the participant.",
                    details = ex.Message
                });
            }
        }

    }
}
