using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SmartHiring.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHiring.Repository.Data.Configurations
{
	public class PostSkillConfiguration : IEntityTypeConfiguration<PostSkill>
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
