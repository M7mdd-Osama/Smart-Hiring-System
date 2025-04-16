using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SmartHiring.Core.Entities;

namespace SmartHiring.Repository.Data.Configurations
{
	public class PostCareerLevelConfiguration : IEntityTypeConfiguration<PostCareerLevel>
	{
		public void Configure(EntityTypeBuilder<PostCareerLevel> builder)
		{
			builder.HasKey(pc => new { pc.PostId, pc.CareerLevelId });

			builder.HasOne(pc => pc.Post)
				.WithMany(p => p.PostCareerLevels)
				.HasForeignKey(pc => pc.PostId);

			builder.HasOne(pc => pc.CareerLevel)
				.WithMany(cl => cl.PostCareerLevels)
				.HasForeignKey(pc => pc.CareerLevelId);
		}
	}
}
