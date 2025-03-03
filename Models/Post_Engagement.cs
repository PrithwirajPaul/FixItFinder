using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FixItFinderDemo.Models
{
    public class Post_Engagement
    {
        [Key]
        public int Id { get; set; }
        public int PostId { get; set; }
        public string Status { get; set; } = null!;
        public int EngagedUserId { get; set; }

        [ForeignKey("PostId")]
        public Post Post { get; set; } = null!;
        [ForeignKey("EngagedUserId")]
        public User EngagedUser { get; set; } = null!;


    }
}
