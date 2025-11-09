using AutoMapper;
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
    [Authorize(Roles = "Admin")] // Only Admin can manage users
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

       
        // ================================================================
        // POST: Create a new user (Admin only)
        // ================================================================
        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateUpdateDto dto, CancellationToken ct)
        {
            //  1. Validate input object (null or invalid model)
            if (dto == null)
                return BadRequest("Invalid user data.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                //  2. Check if username or email already exists
                var exists = await _userService.ExistsByUsernameOrEmailAsync(dto.Username, dto.Email, ct);
                if (exists)
                    return Conflict("Username or Email already exists.");

                //  3. Try creating the user
                var created = await _userService.CreateAsync(dto, ct);
                if (created == null)
                    return StatusCode(500, "Failed to create the user due to an unknown error.");

                //  4. Return success response with 201 Created
                return CreatedAtAction(nameof(GetUserById), new { id = created.Id }, created);
            }
            catch (ArgumentException ex)
            {
                //  Handles specific invalid arguments (e.g., invalid email format, etc.)
                return BadRequest(new
                {
                    Message = "Invalid input detected.",
                    Details = ex.Message
                });
            }
            catch (OperationCanceledException)
            {
                //  Handles request cancellation (e.g., client disconnected)
                return StatusCode(499, "Request was cancelled by the client.");
            }
            catch (Exception ex)
            {
                //  Catch any unexpected system-level exceptions
                // Log the error (you can integrate a logger here later)
                return StatusCode(500, new
                {
                    Message = "An unexpected error occurred while creating the user.",
                    Error = ex.Message
                });
            }
        }


        // ================================================================
        // Get: api/users/GetAllUsers
        // Description: Retrieve all users
        // ================================================================
        [HttpGet("GetAllUsers")] //  Endpoint to get all users
        public async Task<IActionResult> GetAllUsers(CancellationToken ct)
        {
            try
            {
                //  Attempt to retrieve all users from the service
                var users = await _userService.GetAllAsync(ct);

                //  Check if list is empty
                if (users == null || !users.Any())
                {
                    return NotFound(new
                    {
                        Message = "No users found in the system.",
                        Status = false
                    });
                }

                //  Return successful response
                return Ok(new
                {
                    Message = "Users retrieved successfully.",
                    Count = users.Count(),
                    Data = users,
                    Status = true
                });
            }
            catch (OperationCanceledException)
            {
                //  Handle cancellation requests (when token is triggered)
                return BadRequest(new
                {
                    Message = "The request was canceled.",
                    Status = false
                });
            }
            catch (Exception ex)
            {
                //  Handle unexpected errors
                return StatusCode(500, new
                {
                    Message = "An unexpected error occurred while retrieving users.",
                    Error = ex.Message,
                    Status = false
                });
            }
        }


        
        // ================================================================
        // GET: GetUserById
        // Description: Retrieves a user by ID 
        // ================================================================
        [HttpGet("GetUserById")]
        public async Task<ActionResult<UserReadDto>> GetUserById(int id, CancellationToken ct)
        {
            try
            {
                //  1. Validate ID before processing
                if (id <= 0)
                    return BadRequest("Invalid user ID. ID must be greater than zero.");

                //  2. Attempt to get user from service
                var user = await _userService.GetByIdAsync(id, ct);

                //  3. Handle not found case
                if (user is null)
                    return NotFound(new
                    {
                        Message = "User not found.",
                        RequestedId = id
                    });

                //  4. Return success
                return Ok(user);
            }
            catch (ArgumentException ex)
            {
                //  Specific validation errors from service layer
                return BadRequest(new
                {
                    Message = "Invalid input data.",
                    Details = ex.Message
                });
            }
            catch (OperationCanceledException)
            {
                //  Handles request cancellation (e.g., client disconnected)
                return StatusCode(499, "Request was cancelled by the client.");
            }
            catch (Exception ex)
            {
                //  Catch any unexpected error
                // Optional: Log exception using ILogger (can add later)
                return StatusCode(500, new
                {
                    Message = "An unexpected error occurred while retrieving the user.",
                    Error = ex.Message
                });
            }
        }


        // ================================================================
        // PUT: UpdateUser
        // Description: Updates an existing user 
        // ================================================================
        [HttpPut("UpdateUser")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserCreateUpdateDto dto, CancellationToken ct)
        {
            try
            {
                //  1. Validate inputs before calling service
                if (id <= 0)
                    return BadRequest("Invalid user ID. ID must be greater than zero.");

                if (dto == null)
                    return BadRequest("User data is required.");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                //  2. Attempt to update user
                await _userService.UpdateAsync(id, dto, ct);

                //  3. Return success if no exception was thrown
                return Ok(new
                {
                    Message = "User updated successfully.",
                    UpdatedUserId = id
                });
            }
            catch (KeyNotFoundException)
            {
                //  Specific case when user doesn't exist
                return NotFound(new
                {
                    Message = "User not found.",
                    RequestedId = id
                });
            }
            catch (ArgumentException ex)
            {
                //  Handles invalid input or validation errors from service layer
                return BadRequest(new
                {
                    Message = "Invalid data provided.",
                    Details = ex.Message
                });
            }
            catch (OperationCanceledException)
            {
                //  Handles request cancellation (client closed connection, etc.)
                return StatusCode(499, "Request was cancelled by the client.");
            }
            catch (Exception ex)
            {
                //  Catch any unexpected exception (system-level)
                // Optional: can be logged using ILogger
                return StatusCode(500, new
                {
                    Message = "An unexpected error occurred while updating the user.",
                    Error = ex.Message
                });
            }
        }


        
        // ================================================================
        // DELETE: DeleteUser
        // Description: Deletes a user by ID 
        // ================================================================
        [HttpDelete("DeleteUser")]
        public async Task<IActionResult> DeleteUser(int id, CancellationToken ct)
        {
            try
            {
                //  1. Validate ID
                if (id <= 0)
                    return BadRequest("Invalid user ID. ID must be greater than zero.");

                //  2. Attempt to delete user
                await _userService.DeleteAsync(id, ct);

                //  3. Return success if deletion succeeds
                return Ok(new
                {
                    Message = $"User with ID {id} deleted successfully.",
                    DeletedUserId = id
                });
            }
            catch (KeyNotFoundException)
            {
                //  Specific case when user not found in database
                return NotFound(new
                {
                    Message = "User not found.",
                    RequestedId = id
                });
            }
            catch (ArgumentException ex)
            {
                //  Handles invalid data or service-level validation errors
                return BadRequest(new
                {
                    Message = "Invalid input provided.",
                    Details = ex.Message
                });
            }
            catch (OperationCanceledException)
            {
                //  Handles when the request is cancelled by the client
                return StatusCode(499, "Request was cancelled by the client.");
            }
            catch (Exception ex)
            {
                //  Catch-all for any unexpected system errors
                // You can later log this exception using ILogger for debugging
                return StatusCode(500, new
                {
                    Message = "An unexpected error occurred while deleting the user.",
                    Error = ex.Message
                });
            }
        }

    }
}

