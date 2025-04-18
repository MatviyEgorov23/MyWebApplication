using System.ComponentModel.DataAnnotations;

namespace WebApplication_AuthenticationSystem_.Models
{
    public class RegisterViewModel
    {
        [Required]
        public string? FirstName { get; set; }

        [Required]
        public string? LastName { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string? Password { get; set; }

        [Required]
        [Phone]
        public string? PhoneNumber { get; set; }
    }
}
