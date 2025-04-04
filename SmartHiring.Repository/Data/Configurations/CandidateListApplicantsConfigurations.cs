using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SmartHiring.Core.Entities;

namespace SmartHiring.Repository.Data.Configurations
{
	public class CandidateListApplicantsConfigurations : IEntityTypeConfiguration<CandidateListApplicant>
	{
		public void Configure(EntityTypeBuilder<CandidateListApplicant> builder)
		{
			builder.ToTable("CandidateListApplicants");

			builder.HasKey(cla =>new { cla.CandidateListId, cla.ApplicantId });

			builder.HasOne(cla => cla.CandidateList)
				.WithMany(cl => cl.CandidateListApplicants)
				.HasForeignKey(cla => cla.CandidateListId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.HasOne(cla => cla.Applicant)
				.WithMany(a => a.CandidateListApplicants)
				.HasForeignKey(cla => cla.ApplicantId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
