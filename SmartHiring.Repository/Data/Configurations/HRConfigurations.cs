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
	public class HRConfigurations : IEntityTypeConfiguration<HR>
	{
		public void Configure(EntityTypeBuilder<HR> builder)
		{
			builder.ToTable("HRs");

			builder.HasKey(h => h.Id);
			builder.Property(h => h.Id).ValueGeneratedOnAdd();

			builder.Property(h => h.Name).IsRequired().HasMaxLength(100);
			builder.Property(h => h.Email).IsRequired().HasMaxLength(100);
			builder.Property(h => h.Password).IsRequired();
			builder.Property(h => h.Role).IsRequired().HasMaxLength(50);

			builder.HasMany(h => h.Posts)
				   .WithOne(p => p.HR)
				   .HasForeignKey(p => p.HRId)
				   .OnDelete(DeleteBehavior.Cascade);

			builder.Navigation(h => h.Posts).AutoInclude();

			builder.HasOne(h => h.Company)
				   .WithMany(c => c.HRs)
				   .HasForeignKey(h => h.CompanyId);

		}
	}
}
