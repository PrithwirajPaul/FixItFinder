using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text;

namespace FixItFinderDemo.Models
{
    public class User
    {
        [Key]
        public int UId { get; set; }
        public string? Name { get; set; }
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;   
        public string Role { get; set; } = null!;
        public string? City { get; set; }
        public string? Address { get; set; }
        public string? ZipCode { get; set; }
        public string? PhoneNumber { get; set; }
        public List<Post>? Posts { get; set; }
        public Customer_Profile? Customer_Profile { get; set; } // Add this property
        public Worker_Profile? Worker_Profile { get; set; } // Add this property
    }
}
