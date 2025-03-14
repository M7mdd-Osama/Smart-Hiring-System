using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SmartHiring.Core.Entities;

namespace SmartHiring.Repository.Data.Configurations
{
	public class InterviewConfigurations : IEntityTypeConfiguration<Interview>
	{
		public void Configure(EntityTypeBuilder<Interview> builder)
		{
			builder.ToTable("Interviews");

			builder.HasKey(i => i.Id);

			builder.Property(i => i.Date)
				.IsRequired()
				.HasColumnType("date");

			builder.Property(i => i.Time)
				.IsRequired()
				.HasColumnType("time");

			builder.Property(i => i.InterviewStatus)
				.HasConversion<string>()
				.HasColumnType("nvarchar(20)")
				.HasMaxLength(50);

			builder.Property(i => i.Score)
				.HasMaxLength(50)
				.HasDefaultValue(0);

			builder.Property(i => i.Location)
				.HasMaxLength(200);

			builder.HasOne(i => i.HR)
				.WithMany(h => h.Interviews)
				.HasForeignKey(i => i.HRId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.HasOne(i => i.Post)
				.WithMany()
				.HasForeignKey(i => i.PostId)
				.OnDelete(DeleteBehavior.NoAction);

			builder.HasOne(i => i.Applicant)
				.WithMany()
				.HasForeignKey(i => i.ApplicantId)
				.OnDelete(DeleteBehavior.NoAction);
		}
	}
}
