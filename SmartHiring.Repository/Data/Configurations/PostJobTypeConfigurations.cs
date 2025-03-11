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
	public class PostJobTypeConfiguration : IEntityTypeConfiguration<PostJobType>
	{
		public void Configure(EntityTypeBuilder<PostJobType> builder)
		{
			builder.HasKey(pjt => new { pjt.PostId, pjt.JobTypeId });

			builder.HasOne(pjt => pjt.Post)
				.WithMany(p => p.PostJobTypes)
				.HasForeignKey(pjt => pjt.PostId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.HasOne(pjt => pjt.JobType)
				.WithMany(jt => jt.PostJobTypes)
				.HasForeignKey(pjt => pjt.JobTypeId)
				.OnDelete(DeleteBehavior.NoAction);

		}
	}
}