using System.ComponentModel.DataAnnotations;

namespace EnrollmentApp.Models
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string PasswordSalt { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        public string? FullName { get; set; }
        public string Role { get; set; } = "Staff";
    }
}