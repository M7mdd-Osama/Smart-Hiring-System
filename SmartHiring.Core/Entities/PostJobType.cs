using System.ComponentModel.DataAnnotations.Schema;

namespace SmartHiring.Core.Entities
{
	public class PostJobType
	{
		public int PostId { get; set; }
		[ForeignKey("PostId")]
		public Post Post { get; set; }
		public int JobTypeId { get; set; }
		[ForeignKey("JobTypeId")]
		public JobType JobType { get; set; }

	}
}