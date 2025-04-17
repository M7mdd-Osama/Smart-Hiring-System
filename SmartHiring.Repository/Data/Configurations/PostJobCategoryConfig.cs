using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SmartHiring.Core.Entities;

namespace SmartHiring.Repository.Data.Configurations
{
	public class PostJobCategoryConfig : IEntityTypeConfiguration<PostJobCategory>
	{
		public void Configure(EntityTypeBuilder<PostJobCategory> builder)
		{
			builder.HasKey(pjc => new { pjc.PostId, pjc.JobCategoryId });

			builder.HasOne(pjc => pjc.Post)
				.WithMany(p => p.PostJobCategories)
				.HasForeignKey(pjc => pjc.PostId);

			builder.HasOne(pjc => pjc.JobCategory)
				.WithMany(jc => jc.PostJobCategories)
				.HasForeignKey(pjc => pjc.JobCategoryId);
		}
	}
}
