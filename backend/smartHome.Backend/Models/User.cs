using System;

namespace SmartHome.backend.Models
{
    public class User
    {
        public int Id { get; set; }
        
        public string Email { get; set; } = string.Empty;
        
        public string HashedPassword { get; set; } = string.Empty;

        public string Role { get; set; } = "User";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
