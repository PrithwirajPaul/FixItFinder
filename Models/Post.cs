using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FixItFinderDemo.Models
{
    public class Post
    {
        [Key]
        public int PId { get; set; }
        public int Post_Status { get; set; }
        public string? Description { get; set; }
        public int Views { get; set; }

        public int Price { get; set; } 

        [Required]
        public int? UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }
        public List<Post_Engagement>? PostEngagements { get; set; }
    }
}
