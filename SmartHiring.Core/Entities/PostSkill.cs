using System.ComponentModel.DataAnnotations.Schema;

namespace SmartHiring.Core.Entities
{
	public class PostSkill
	{
		public int PostId { get; set; }
		[ForeignKey("PostId")]
		public Post Post { get; set; }

		public int SkillId { get; set; }
		[ForeignKey("SkillId")]
		public Skill Skill { get; set; }

	}
}