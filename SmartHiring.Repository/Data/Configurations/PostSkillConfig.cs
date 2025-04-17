using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SmartHiring.Core.Entities;

namespace SmartHiring.Repository.Data.Configurations
{
	public class PostSkillConfig : IEntityTypeConfiguration<PostSkill>
	{
		public void Configure(EntityTypeBuilder<PostSkill> builder)
		{
			builder.HasKey(ps => new { ps.PostId, ps.SkillId });

			builder.HasOne(ps => ps.Post)
				.WithMany(p => p.PostSkills)
				.HasForeignKey(ps => ps.PostId);

			builder.HasOne(ps => ps.Skill)
				.WithMany(s => s.PostSkills)
				.HasForeignKey(ps => ps.SkillId);
		}
	}
}
