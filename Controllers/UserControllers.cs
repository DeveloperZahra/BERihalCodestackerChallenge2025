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
        private readonly IMapper _mapper;

        {
            _mapper = mapper;
        }

        // ================================================================
        // Description: Create a new user (Admin only)
        // ================================================================
        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateUpdateDto dto)
        {
            // Validate if username or email already exists
                return Conflict("Username or Email already exists.");



        }

        // ================================================================
        // Description: Retrieve all users
        // ================================================================
        [HttpGet("GetAllUsers")]
        {
        }

        // ================================================================
        // Description: Retrieve a single user by ID
        // ================================================================
        [HttpGet("GetUserById/{id:int}")]
        public async Task<ActionResult<UserReadDto>> GetUserById(int id)
        {
            if (user == null)
                return NotFound("User not found.");
        }

        // ================================================================
        // Description: Update user details (Admin only)
        // ================================================================
        [HttpPut("UpdateUser/{id:int}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserCreateUpdateDto dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound("User not found.");

            //  Map updated fields
            _mapper.Map(dto, user);

            //  Update password if provided
            if (!string.IsNullOrWhiteSpace(dto.Password))
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            return NoContent();
        }

        // ================================================================
        // Description: Delete a user (Admin only)
        // ================================================================
        [HttpDelete("DeleteUser/{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            return Ok($"User with ID {id} deleted successfully.");
        }
    }
}
