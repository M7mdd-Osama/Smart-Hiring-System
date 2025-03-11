using System.ComponentModel.DataAnnotations.Schema;

namespace SmartHiring.Core.Entities
{
	public class PostCareerLevel
	{
		public int PostId { get; set; }
		[ForeignKey("PostId")]
		public Post Post { get; set; }

		public int CareerLevelId { get; set; }
		[ForeignKey("CareerLevelId")]
		public CareerLevel CareerLevel { get; set; }
	}
}