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
	public class ApplicantPhoneConfigurations : IEntityTypeConfiguration<ApplicantPhone>
	{
		public void Configure(EntityTypeBuilder<ApplicantPhone> builder)
		{
			builder.ToTable("Applicant_Phones");

			builder.HasKey(ap => new { ap.ApplicantId, ap.Phone });

			builder.HasOne(ap => ap.Applicant)
				.WithMany(a => a.ApplicantPhones)
				.HasForeignKey(ap => ap.ApplicantId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.Property(ap => ap.Phone)
				.IsRequired()
				.HasMaxLength(20);
		}
	}
}
