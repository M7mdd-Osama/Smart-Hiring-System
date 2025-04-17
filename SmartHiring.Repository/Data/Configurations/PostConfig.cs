using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SmartHiring.Core.Entities;

namespace SmartHiring.Repository.Data.Configurations
{
	public class PostConfig : IEntityTypeConfiguration<Post>
	{
		public void Configure(EntityTypeBuilder<Post> builder)
		{
			builder.ToTable("Posts");

			builder.HasKey(p => p.Id);

			builder.Property(p => p.JobTitle)
				.IsRequired()
				.HasMaxLength(150);

			builder.Property(p => p.Description)
				.IsRequired();

			builder.Property(p => p.Requirements)
				.IsRequired();

			builder.Property(p => p.PostDate)
				.IsRequired();

			builder.Property(p => p.Deadline)
				.IsRequired();


			builder.Property(p => p.MinSalary)
			.HasColumnType("decimal(18,2)");

			builder.Property(p => p.MaxSalary)
			.HasColumnType("decimal(18,2)");

			builder.HasOne(p => p.HR)
				.WithMany(h => h.Posts)
				.HasForeignKey(p => p.HRId)
				.OnDelete(DeleteBehavior.Cascade);

		}
	}
}
