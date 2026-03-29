using Microsoft.AspNetCore.Mvc;
using TodoApi.DTOs;
using TodoApi.Services;

namespace TodoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new AuthResponseDto { Success = false, Message = "Invalid input" });

                var (success, message, token, user) = await _authService.RegisterAsync(dto.Email, dto.Password, dto.FullName);

                if (!success)
                    return BadRequest(new AuthResponseDto { Success = false, Message = message });

                var response = new AuthResponseDto
                {
                    Success = true,
                    Message = message,
                    Token = token,
                    User = new UserResponseDto
                    {
                        Id = user?.Id,
                        Email = user?.Email ?? string.Empty,
                        FullName = user?.FullName ?? string.Empty
                    }
                };

                _logger.LogInformation($"User registered: {dto.Email}");
                return CreatedAtAction(nameof(Register), response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration");
                return StatusCode(500, new AuthResponseDto
                {
                    Success = false,
                    Message = "An error occurred during registration"
                });
            }
        }

        /// <summary>
        /// Login a user
        /// </summary>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new AuthResponseDto { Success = false, Message = "Invalid input" });

                var (success, message, token, user) = await _authService.LoginAsync(dto.Email, dto.Password);

                if (!success)
                    return Unauthorized(new AuthResponseDto { Success = false, Message = message });

                var response = new AuthResponseDto
                {
                    Success = true,
                    Message = message,
                    Token = token,
                    User = new UserResponseDto
                    {
                        Id = user?.Id,
                        Email = user?.Email ?? string.Empty,
                        FullName = user?.FullName ?? string.Empty
                    }
                };

                _logger.LogInformation($"User logged in: {dto.Email}");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return StatusCode(500, new AuthResponseDto
                {
                    Success = false,
                    Message = "An error occurred during login"
                });
            }
        }
    }
}
