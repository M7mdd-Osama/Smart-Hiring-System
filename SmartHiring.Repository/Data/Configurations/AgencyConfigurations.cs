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
	public class AgencyConfigurations : IEntityTypeConfiguration<Agency>
	{
		public void Configure(EntityTypeBuilder<Agency> builder)
		{
			builder.ToTable("Agencies");

			builder.HasKey(a => a.Id);
			builder.Property(a => a.Id).ValueGeneratedOnAdd();

			builder.Property(a => a.Name).IsRequired().HasMaxLength(100);
			builder.Property(a => a.Address).IsRequired().HasMaxLength(200);
			builder.Property(a => a.Email).IsRequired().HasMaxLength(100);
			builder.Property(a => a.Phone).IsRequired().HasMaxLength(20);
			builder.Property(a => a.Password).IsRequired();
			builder.Property(a => a.Role).IsRequired().HasMaxLength(20);

			builder.HasMany(a => a.Applications)
				   .WithOne(app => app.Agency)
				   .HasForeignKey(app => app.AgencyId)
				   .OnDelete(DeleteBehavior.Cascade);

			builder.Navigation(a => a.Applications).AutoInclude();
		}
	}
}
