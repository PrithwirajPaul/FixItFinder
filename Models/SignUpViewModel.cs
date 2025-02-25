using System.ComponentModel.DataAnnotations;

namespace FixItFinderDemo.Models
{
    public class SignUpViewModel
    {
        [Required]
        public string Name { get; set; } = null!;

        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required, MinLength(6)]
        public string Password { get; set; } = null!;

        [Required]
        public string PhoneNumber { get; set; } = null!;

        [Required]
        public string Address { get; set; } = null!;

        [Required]
        public string City { get; set; } = null!;

        [Required]
        public string ZipCode { get; set; } = null!;

        [Required]
        public string Role { get; set; } = null!;

        public string? Category { get; set; }

        public int? Experience { get; set; }
    }
}
