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
	public class CompanyPhoneConfigurations : IEntityTypeConfiguration<CompanyPhone>
	{
		public void Configure(EntityTypeBuilder<CompanyPhone> builder)
		{
			builder.ToTable("Company_Phones");

			builder.HasKey(cp => new { cp.CompanyId, cp.Phone });

			builder.HasOne(cp => cp.Company)
				.WithMany(c => c.CompanyPhones)
				.HasForeignKey(cp => cp.CompanyId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.Property(cp => cp.Phone)
				.IsRequired()
				.HasMaxLength(20);
		}
	}
}
