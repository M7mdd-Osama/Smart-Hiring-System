using System.ComponentModel.DataAnnotations.Schema;

namespace SmartHiring.Core.Entities
{
	public class PostWorkplace
	{
		public int PostId { get; set; }
		[ForeignKey("PostId")]
		public Post Post { get; set; }
		public int WorkplaceId { get; set; }
		[ForeignKey("WorkplaceId")]
		public Workplace Workplace { get; set; }

	}
}