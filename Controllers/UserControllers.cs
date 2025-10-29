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
    [Authorize(Roles = "Admin")] // Only Admin can manage users
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public UsersController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // ================================================================
        // POST: api/users
        // Description: Create a new user (Admin only)
        // ================================================================
        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateUpdateDto dto)
        {
            // Validate if username or email already exists
            if (await _context.Users.AnyAsync(u => u.Username == dto.Username || u.Email == dto.Email))
                return Conflict("Username or Email already exists.");

            // Map DTO → Model
            var user = _mapper.Map<User>(dto);

            // Hash password before saving
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            user.CreatedAt = DateTime.UtcNow;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var result = _mapper.Map<UserReadDto>(user);
            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, result);
        }

        // ================================================================
        // GET: api/users
        // Description: Retrieve all users
        // ================================================================
        [HttpGet("GetAllUsers")]
        public async Task<ActionResult<IEnumerable<UserReadDto>>> GetAllUsers()
        {
            var users = await _context.Users.AsNoTracking().ToListAsync();
            return Ok(_mapper.Map<IEnumerable<UserReadDto>>(users));
        }

        // ================================================================
        // GET: api/users/{id}
        // Description: Retrieve a single user by ID
        // ================================================================
        [HttpGet("GetUserById/{id:int}")]
        public async Task<ActionResult<UserReadDto>> GetUserById(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound("User not found.");
            return Ok(_mapper.Map<UserReadDto>(user));
        }

        // ================================================================
        // PUT: api/users/{id}
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

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ================================================================
        // DELETE: api/users/{id}
        // Description: Delete a user (Admin only)
        // ================================================================
        [HttpDelete("DeleteUser/{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound("User not found.");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok($"User with ID {id} deleted successfully.");
        }
    }
}
