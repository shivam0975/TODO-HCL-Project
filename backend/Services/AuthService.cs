using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TodoApi.Models;
using TodoApi.Repositories;

namespace TodoApi.Services
{
    public interface IAuthService
    {
        Task<(bool Success, string Message, string? Token, User? User)> RegisterAsync(string email, string password, string fullName);
        Task<(bool Success, string Message, string? Token, User? User)> LoginAsync(string email, string password);
        string GenerateJwt(User user);
        bool VerifyPassword(string password, string hash);
        string HashPassword(string password);
    }

    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IUserRepository userRepository, IConfiguration configuration, ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<(bool Success, string Message, string? Token, User? User)> RegisterAsync(
            string email, string password, string fullName)
        {
            try
            {
                // Check if user exists
                if (await _userRepository.UserExistsAsync(email))
                {
                    return (false, "User with this email already exists", null, null);
                }

                // Hash password
                var passwordHash = HashPassword(password);

                // Create user
                var user = new User
                {
                    Email = email,
                    PasswordHash = passwordHash,
                    FullName = fullName,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // Save to database
                var createdUser = await _userRepository.CreateUserAsync(user);

                // Generate JWT
                var token = GenerateJwt(createdUser);

                _logger.LogInformation($"User registered successfully: {email}");
                return (true, "User registered successfully", token, createdUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration");
                return (false, "An error occurred during registration", null, null);
            }
        }

        public async Task<(bool Success, string Message, string? Token, User? User)> LoginAsync(
            string email, string password)
        {
            try
            {
                // Find user by email
                var user = await _userRepository.GetUserByEmailAsync(email);
                if (user == null)
                {
                    return (false, "Invalid email or password", null, null);
                }

                // Verify password
                if (!VerifyPassword(password, user.PasswordHash))
                {
                    return (false, "Invalid email or password", null, null);
                }

                // Generate JWT
                var token = GenerateJwt(user);

                _logger.LogInformation($"User logged in successfully: {email}");
                return (true, "Login successful", token, user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return (false, "An error occurred during login", null, null);
            }
        }

        public string GenerateJwt(User user)
        {
            var jwtKey = _configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("JWT Key not configured");
            var jwtIssuer = _configuration["Jwt:Issuer"] ?? "TodoApi";
            var jwtAudience = _configuration["Jwt:Audience"] ?? "TodoApiUsers";

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FullName)
            };

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public bool VerifyPassword(string password, string hash)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hash);
            }
            catch
            {
                return false;
            }
        }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
        }
    }
}
