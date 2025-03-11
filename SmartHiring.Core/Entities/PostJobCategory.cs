using System.ComponentModel.DataAnnotations.Schema;

namespace SmartHiring.Core.Entities
{
	public class PostJobCategory
	{
		public int PostId { get; set; }
		[ForeignKey("PostId")]
		public Post Post { get; set; }

		public int JobCategoryId { get; set; }
		[ForeignKey("JobCategoryId")]
		public JobCategory JobCategory { get; set; }
	}
}