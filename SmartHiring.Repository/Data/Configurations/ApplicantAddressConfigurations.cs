using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SmartHiring.Core.Entities;

namespace SmartHiring.Repository.Data.Configurations
{
	public class ApplicantAddressConfiguration : IEntityTypeConfiguration<ApplicantAddress>
	{
		public void Configure(EntityTypeBuilder<ApplicantAddress> builder)
		{
			builder.ToTable("Applicant_Addresses");

			builder.HasKey(aa => new { aa.ApplicantId, aa.Country });

			builder.HasOne(aa => aa.Applicant)
				.WithMany(a => a.ApplicantAddresses)
				.HasForeignKey(aa => aa.ApplicantId);

			builder.Property(aa => aa.Country)
				.IsRequired()
				.HasMaxLength(100);

			builder.Property(aa => aa.City)
				.IsRequired()
				.HasMaxLength(100);

			builder.Property(aa => aa.ZIP)
				.HasMaxLength(20)
				.IsRequired(false);
		}
	}
}
