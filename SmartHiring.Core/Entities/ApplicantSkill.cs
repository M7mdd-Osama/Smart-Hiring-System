namespace SmartHiring.Core.Entities
{
	public class ApplicantSkill
	{
		public int ApplicantId { get; set; }
		public Applicant Applicant { get; set; }

		public int SkillId { get; set; }
		public Skill Skill { get; set; }
	}
}