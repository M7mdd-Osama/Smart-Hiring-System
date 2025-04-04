using System.ComponentModel.DataAnnotations.Schema;
using SmartHiring.Core.Entities.Identity;

namespace SmartHiring.Core.Entities
{
    public class Note : BaseEntity
    {
        public string Header { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public AppUser User { get; set; }

        public bool IsSeen { get; set; } = false;
        public int PostId { get; set; }
        [ForeignKey("PostId")]
        public Post Post { get; set; }
    }
}
