using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SmartHiring.Core.Entities;

namespace SmartHiring.Repository.Data.Configurations
{
	public class PostWorkplaceConfig : IEntityTypeConfiguration<PostWorkplace>
	{
		public void Configure(EntityTypeBuilder<PostWorkplace> builder)
		{
			builder.HasKey(pw => new { pw.PostId, pw.WorkplaceId });

			builder.HasOne(pw => pw.Post)
				.WithMany(p => p.PostWorkplaces)
				.HasForeignKey(pw => pw.PostId);

			builder.HasOne(pw => pw.Workplace)
				.WithMany(w => w.PostWorkplaces)
				.HasForeignKey(pw => pw.WorkplaceId);
		}
	}
}
