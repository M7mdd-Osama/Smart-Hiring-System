using SmartHiring.Core.Entities.Identity;

namespace SmartHiring.Core.Entities
{
	public class SavedPost
	{
		public string UserId { get; set; }
		public AppUser User { get; set; }
		public int PostId { get; set; }
		public Post Post { get; set; }
	}
}
