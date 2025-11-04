using AutoMapper;
using BERihalCodestackerChallenge2025.DTOs;
using BERihalCodestackerChallenge2025.Model;
using BERihalCodestackerChallenge2025.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BERihalCodestackerChallenge2025.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")] // Only Admin can manage users
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UsersController(IUserService service, IMapper mapper)
        {
            _userService = service;
            _mapper = mapper;
        }

        // ================================================================
        // POST: api/users/CreateUser
        // Description: Create a new user (Admin only)
        // ================================================================
        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateUpdateDto dto)
        {
            // Validate if username or email already exists 
            var exists = await _userService.ExistsByUsernameOrEmailAsync(dto.Username, dto.Email);
            if (exists)
                return Conflict("Username or Email already exists.");

            // Map + hash + save 
            var createdUser = await _userService.CreateAsync(dto);

            // If the service returns Entity:
            var result = _mapper.Map<UserReadDto>(createdUser);

            return CreatedAtAction(nameof(GetUserById), new { id = result.Id }, result);
        }

        // ================================================================
        // GET: api/users/GetAllUsers
        // Description: Retrieve all users
        // ================================================================
        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users); //
        }

        // ================================================================
        // GET: api/users/GetUserById/{id}
        // Description: Retrieve a single user by ID
        // ================================================================
        [HttpGet("GetUserById/{id:int}")]
        public async Task<ActionResult<UserReadDto>> GetUserById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
                return NotFound("User not found.");

            // If the service returns Entity:
            var dto = _mapper.Map<UserReadDto>(user);
            return Ok(dto);
        }

        // ================================================================
        // PUT: api/users/UpdateUser/{id}
        // Description: Update user details (Admin only)
        // ================================================================
        [HttpPut("UpdateUser/{id:int}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserCreateUpdateDto dto)
        {

            await _userService.UpdateAsync(id, dto);
            return NoContent();
        }

        // ================================================================
        // DELETE: api/users/DeleteUser/{id}
        // Description: Delete a user (Admin only)
        // ================================================================
        [HttpDelete("DeleteUser/{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await _userService.DeleteAsync(id); 
            return Ok($"User with ID {id} deleted successfully.");
        }
    }
}
