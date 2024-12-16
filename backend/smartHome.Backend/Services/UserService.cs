using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SmartHome.backend.Data;
using SmartHome.backend.Models;
using BCrypt.Net;

namespace SmartHome.backend.Services
{
    public interface IUserService
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task<User> CreateUserAsync(string email, string password, string role = "User");
        Task<bool> CheckEmailExistsAsync(string email);
    }

    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> CheckEmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<User> CreateUserAsync(string email, string password, string role = "User")
        {
            // Hash the password
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            var user = new User
            {
                Email = email,
                HashedPassword = hashedPassword,
                Role = role
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
    }
}