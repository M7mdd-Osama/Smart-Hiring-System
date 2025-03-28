using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SmartHiring.Core.Entities;

namespace SmartHiring.Repository.Data.Configurations
{
	public class SavedPostsConfigurations : IEntityTypeConfiguration<SavedPost>
	{
		public void Configure(EntityTypeBuilder<SavedPost> builder)
		{
			builder.HasKey(sp => new { sp.UserId, sp.PostId });

			builder.HasOne(sp => sp.User)
				.WithMany(u => u.SavedPosts)
				.HasForeignKey(sp => sp.UserId)
				.OnDelete(DeleteBehavior.SetNull);

			builder.HasOne(sp => sp.Post)
				.WithMany(p => p.SavedPosts)
				.HasForeignKey(sp => sp.PostId)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}
