using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SmartHiring.Core.Entities;

namespace SmartHiring.Repository.Data.Configurations
{
	public class ApplicantConfigurations : IEntityTypeConfiguration<Applicant>
	{
		public void Configure(EntityTypeBuilder<Applicant> builder)
		{
			builder.ToTable("Applicants");

			builder.HasKey(a => a.Id);

			builder.Property(a => a.FName)
				.IsRequired()
				.HasMaxLength(100);

			builder.Property(a => a.LName)
				.IsRequired()
				.HasMaxLength(100);

			builder.Property(a => a.Email)
				.IsRequired()
				.HasMaxLength(150);

			builder.Property(a => a.DateOfBirth)
				.IsRequired(false)
				.HasColumnType("date");

			builder.HasMany(a => a.ApplicantAddresses)
				.WithOne(a => a.Applicant)
				.HasForeignKey(a => a.ApplicantId);

			builder.HasMany(a => a.CandidateListApplicants)
				   .WithOne(cla => cla.Applicant)
				   .HasForeignKey(cla => cla.ApplicantId);
		}
	}
}
