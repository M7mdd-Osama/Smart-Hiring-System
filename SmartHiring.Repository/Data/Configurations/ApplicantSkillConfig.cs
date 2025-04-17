using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SmartHiring.Core.Entities;

namespace SmartHiring.Repository.Data.Configurations
{
	public class ApplicantSkillConfig: IEntityTypeConfiguration<ApplicantSkill>
	{
		public void Configure(EntityTypeBuilder<ApplicantSkill> builder)
		{
			builder.HasKey(AS => new { AS.ApplicantId, AS.SkillId });

			builder.HasOne(AS => AS.Applicant)
				   .WithMany(a => a.ApplicantSkills)
				   .HasForeignKey(AS => AS.ApplicantId)
				   .OnDelete(DeleteBehavior.Cascade);

			builder.HasOne(AS => AS.Skill)
				   .WithMany(s => s.ApplicantSkills)
				   .HasForeignKey(AS => AS.SkillId)
				   .OnDelete(DeleteBehavior.Cascade);
		}
	}
}
