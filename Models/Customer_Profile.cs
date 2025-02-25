using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FixItFinderDemo.Models
{
    public class Customer_Profile
    {
        public int Id { get; set; }
        public string? Image { get; set; }
        public List<string>? Bio { get; set; }
        
        public List<Service_History>? Service_Histories { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }
    }
}
