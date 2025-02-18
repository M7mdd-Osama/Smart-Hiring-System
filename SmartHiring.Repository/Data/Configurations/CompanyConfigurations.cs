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
	public class CompanyConfigurations : IEntityTypeConfiguration<Company>
	{
		public void Configure(EntityTypeBuilder<Company> builder)
		{
			builder.ToTable("Companies");

			builder.HasKey(c => c.Id);

			builder.Property(c => c.Name)
				.IsRequired()
				.HasMaxLength(200);

			builder.Property(c => c.Location)
				.HasMaxLength(250);

			builder.Property(c => c.BusinessEmail)
				.IsRequired()
				.HasMaxLength(150);

			builder.HasOne(c => c.Admin)
				.WithMany(a => a.Companies)
				.HasForeignKey(c => c.AdminId)
				.OnDelete(DeleteBehavior.NoAction);

			builder.HasOne(c => c.Manager)
				.WithOne(a => a.Company)
				.HasForeignKey<Company>(c => c.ManagerId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.HasMany(c => c.HRs)
				.WithOne(h => h.Company)
				.HasForeignKey(h => h.CompanyId);

			builder.HasMany(c => c.CompanyPhones)
				.WithOne()
				.HasForeignKey(p => p.CompanyId);
		}
	}
}
