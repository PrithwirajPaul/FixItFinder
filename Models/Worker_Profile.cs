using System.ComponentModel.DataAnnotations.Schema;

namespace FixItFinderDemo.Models
{
    public class Worker_Profile
    {
        public int Id { get; set; }
        public string? Image { get; set; }
        public List<string>? Bio { get; set; }
        public string Category { get; set; } = null!;
        public int Experience { get; set; } 
        public float? Rating { get; set; }
        
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }
        public List<Service_History>? Service_Histories { get; set; } 
    }
}
