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
        // Description: Create a new user (Admin only)
        // ================================================================
        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateUpdateDto dto, CancellationToken ct)
        {
            
            var exists = await _userService.ExistsByUsernameOrEmailAsync(dto.Username, dto.Email, ct);
            if (exists)
                return Conflict("Username or Email already exists.");

            try
            {
                var created = await _userService.CreateAsync(dto, ct);
             
                return CreatedAtAction(nameof(GetUserById), new { id = created.Id }, created);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // ================================================================
        // POST: api/users/GetAllUsers
        // Description: Retrieve all users
        // ================================================================
        [HttpPost("GetAllUsers")] // Using POST to allow complex filtering in future
        public async Task<IActionResult> GetAllUsers(CancellationToken ct)
        {
            var users = await _userService.GetAllAsync(ct);
            return Ok(users);
        }

        // ================================================================
        // GET: api/users/GetUserById/{id}
        // Description: Retrieve a single user by ID
        // ================================================================
        [HttpGet("GetUserById/{id:int}")]
        public async Task<ActionResult<UserReadDto>> GetUserById(int id, CancellationToken ct)
        {
            var user = await _userService.GetByIdAsync(id, ct);
            if (user is null)
                return NotFound("User not found.");
            return Ok(user);
        }

        // ================================================================
        // PUT: api/users/UpdateUser/{id}
        // Description: Update user details (Admin only)
        // ================================================================
        [HttpPut("UpdateUser/{id:int}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserCreateUpdateDto dto, CancellationToken ct)
        {
            try
            {
                await _userService.UpdateAsync(id, dto, ct);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound("User not found.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // ================================================================
        // DELETE: api/users/DeleteUser/{id}
        // Description: Delete a user (Admin only)
        // ================================================================
        [HttpDelete("DeleteUser/{id:int}")]
        public async Task<IActionResult> DeleteUser(int id, CancellationToken ct)
        {
            try
            {
                await _userService.DeleteAsync(id, ct);
                return Ok($"User with ID {id} deleted successfully.");
            }
            catch (KeyNotFoundException)
            {
                return NotFound("User not found.");
            }
        }
    }
}
