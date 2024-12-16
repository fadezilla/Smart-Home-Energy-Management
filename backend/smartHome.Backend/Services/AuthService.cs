using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SmartHome.backend.Models;

namespace SmartHome.backend.Services
{
    public interface IAuthService
    {
        Task<User?> RegisterAsync(string email, string password, string role = "User");
        Task<string?> LoginAsync(string email, string password);
    }

    public class AuthService : IAuthService
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public AuthService(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

        public async Task<User?> RegisterAsync(string email, string password, string role = "User")
        {
            var emailExists = await _userService.CheckEmailExistsAsync(email);
            if (emailExists)
            {
                // Email already in use
                return null;
            }

            var user = await _userService.CreateUserAsync(email, password, role);
            return user;
        }

        public async Task<string?> LoginAsync(string email, string password)
        {
            var user = await _userService.GetUserByEmailAsync(email);
            if (user == null)
            {
                return null; // User not found
            }

            // Verify password
            bool isValid = BCrypt.Net.BCrypt.Verify(password, user.HashedPassword);
            if (!isValid)
            {
                return null; // Invalid password
            }

            // Generate JWT
            return GenerateJwtToken(user);
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
