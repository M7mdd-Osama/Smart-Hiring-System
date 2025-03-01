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

			builder.Property(c => c.BusinessEmail)
				.IsRequired()
				.HasMaxLength(150);

			builder.Property(c => c.LogoUrl)
				.HasMaxLength(500)
				.IsUnicode(false);

			builder.HasOne(c => c.Manager)
				.WithOne(a => a.ManagedCompany)
				.HasForeignKey<Company>(c => c.ManagerId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.HasOne(c => c.HR)
				.WithOne(h => h.HRCompany)
				.HasForeignKey<Company>(h => h.HRId)
				.OnDelete(DeleteBehavior.NoAction);


			builder.HasMany(c => c.CompanyPhones)
				.WithOne()
				.HasForeignKey(p => p.CompanyId);
		}
	}
}
