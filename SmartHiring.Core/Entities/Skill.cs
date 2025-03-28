namespace SmartHiring.Core.Entities
{
	public class Skill : BaseEntity
	{
		public string SkillName { get; set; }
		public ICollection<PostSkill> PostSkills { get; set; } = new HashSet<PostSkill>();
		public ICollection<ApplicantSkill> ApplicantSkills { get; set; } = new HashSet<ApplicantSkill>();
	}
}
