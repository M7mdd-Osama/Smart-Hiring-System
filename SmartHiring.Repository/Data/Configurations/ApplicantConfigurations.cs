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
				.IsRequired()
				.HasColumnType("date");

			builder.HasMany(a => a.ApplicantPhones)
				.WithOne()
				.HasForeignKey(p => p.ApplicantId);

			builder.HasMany(a => a.ApplicantAddresses)
				.WithOne(a => a.Applicant)
				.HasForeignKey(a => a.ApplicantId);

			builder.HasMany(a => a.CandidateListApplicants)
				   .WithOne(cla => cla.Applicant)
				   .HasForeignKey(cla => cla.ApplicantId);
		}
	}
}
