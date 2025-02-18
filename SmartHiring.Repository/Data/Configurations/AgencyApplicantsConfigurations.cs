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
	public class AgencyApplicantsConfigurations : IEntityTypeConfiguration<AgencyApplicant>
	{
		public void Configure(EntityTypeBuilder<AgencyApplicant> builder)
		{
			builder.ToTable("AgencyApplicants");

			builder.HasKey(aa => new { aa.AgencyId, aa.ApplicantId });

			builder.HasOne(aa => aa.Agency)
				.WithMany(a => a.AgencyApplicants)
				.HasForeignKey(aa => aa.AgencyId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.HasOne(aa => aa.Applicant)
				.WithMany()
				.HasForeignKey(aa => aa.ApplicantId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
