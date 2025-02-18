using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHiring.Core.Entities;

namespace SmartHiring.Repository.Data.Configurations
{
	public class ApplicationConfigurations : IEntityTypeConfiguration<Application>
	{
		public void Configure(EntityTypeBuilder<Application> builder)
		{
			builder.ToTable("Applications");

			builder.HasKey(app => app.Id);
			builder.Property(app => app.Id).ValueGeneratedOnAdd();

			builder.Property(app => app.RankScore).IsRequired();
			builder.Property(app => app.IsShortlisted).IsRequired();
			builder.Property(app => app.ApplicationDate).IsRequired();
			builder.Property(app => app.CV_Link).IsRequired().HasMaxLength(500);

			builder.HasOne(app => app.Agency)
				   .WithMany(a => a.Applications)
				   .HasForeignKey(app => app.AgencyId)
				   .OnDelete(DeleteBehavior.Cascade);

			builder.HasOne(a => a.Applicant)
				   .WithMany(a => a.Applications)
				   .HasForeignKey(a => a.ApplicantId)
				   .OnDelete(DeleteBehavior.Cascade);

			builder.HasOne(a => a.Post)
				   .WithMany(p => p.Applications)
				   .HasForeignKey(a => a.PostId)
				   .OnDelete(DeleteBehavior.NoAction);
		}
	}
}
