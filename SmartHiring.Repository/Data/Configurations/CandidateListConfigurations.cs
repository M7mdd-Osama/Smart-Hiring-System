using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SmartHiring.Core.Entities;

namespace SmartHiring.Repository.Data.Configurations
{
	public class CandidateListConfigurations : IEntityTypeConfiguration<CandidateList>
	{
		public void Configure(EntityTypeBuilder<CandidateList> builder)
		{
			builder.ToTable("CandidateLists");

			builder.HasKey(cl => cl.Id);
			builder.Property(cl => cl.Id).ValueGeneratedOnAdd();

			builder.Property(cl => cl.GeneratedDate).IsRequired();
			builder.Property(cl => cl.Status).IsRequired().HasMaxLength(50);

			builder.HasOne(cl => cl.Manager)
				   .WithMany()
				   .HasForeignKey(cl => cl.ManagerId)
				   .OnDelete(DeleteBehavior.NoAction);

			builder.HasOne(cl => cl.Post)
				   .WithMany(p => p.CandidateLists)
				   .HasForeignKey(cl => cl.PostId)
				   .OnDelete(DeleteBehavior.Cascade);
		}
	}
}
