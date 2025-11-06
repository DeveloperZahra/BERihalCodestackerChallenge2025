using BERihalCodestackerChallenge2025.DTOs;
using BERihalCodestackerChallenge2025.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BERihalCodestackerChallenge2025.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly JwtService _jwtService;

        public AuthController(IUserService userService, JwtService jwtService)
        {
            _userService = userService;
            _jwtService = jwtService;
        }

        // ============================================================
        // Register a new user
        // ============================================================
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] UserCreateUpdateDto dto)
        {
            try
            {
                // Checking for a user with the same name or email
                var exists = await _userService.ExistsByUsernameOrEmailAsync(dto.Username, dto.Email);
                if (exists)
                    return BadRequest(new { message = "Username or Email already exists." });

                // Create a new user
                var user = await _userService.CreateAsync(new UserCreateUpdateDto
                {
                    Username = dto.Username,
                    Email = dto.Email,
                    Password = dto.Password,
                    Role = dto.Role ?? "User",
                    ClearanceLevel = dto.ClearanceLevel ?? "Low"
                });

                return Ok(new
                {
                    message = "User registered successfully.",
                    user.Username,
                    user.Email,
                    user.Role,
                    user.ClearanceLevel
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ============================================================
        //  Login and generate JWT token
        // ============================================================
        [HttpGet("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _userService.ValidateCredentialsAsync(dto.UsernameOrEmail, dto.Password);

            if (user == null)
                return Unauthorized(new { message = "Invalid username or password" });

            // Generating JWT Token
            var token = _jwtService.GenerateToken(user);

            return Ok(new
            {
                message = "Login successful",
                token,
                user.Username,
                user.Role,
                user.ClearanceLevel
            });
        }
    }

    
}
