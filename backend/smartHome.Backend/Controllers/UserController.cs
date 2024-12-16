using Microsoft.AspNetCore.Mvc;
using SmartHome.backend.Dto;
using SmartHome.backend.Services;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace SmartHome.backend.Controllers
{
    /// <summary>
    /// Handles user registration and login operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IAuthService _authService;

        public UserController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Registers a new user with a given email and password.
        /// </summary>
        /// <param name="dto">The registration details (email and password).</param>
        /// <returns>Status 201 if successful, 400 if email already taken.</returns>
        /// <response code="201">User created successfully</response>
        /// <response code="400">Email already exists or invalid data</response>
        [HttpPost("signup")]
        [SwaggerResponse(201, "User created successfully")]
        [SwaggerResponse(400, "Email already exists or invalid data")]
        public async Task<IActionResult> SignUp([FromBody] RegisterDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            {
                return BadRequest("Email and password are required.");
            }

            var user = await _authService.RegisterAsync(dto.Email, dto.Password);
            if (user == null)
            {
                return BadRequest("Email already in use.");
            }

            return StatusCode(201, "User registered successfully.");
        }

        /// <summary>
        /// Logs in an existing user and returns a JWT token.
        /// </summary>
        /// <param name="dto">The login details (email and password).</param>
        /// <returns>A JWT token if successful, 401 if invalid credentials.</returns>
        /// <response code="200">Login successful, returns JWT token</response>
        /// <response code="401">Invalid credentials</response>
        [HttpPost("login")]
        [SwaggerResponse(200, "Login successful, returns JWT token")]
        [SwaggerResponse(401, "Invalid credentials")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            {
                return BadRequest("Email and password are required.");
            }

            var token = await _authService.LoginAsync(dto.Email, dto.Password);
            if (token == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            return Ok(new { token });
        }
    }
}
