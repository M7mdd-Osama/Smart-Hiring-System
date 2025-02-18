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
	public class ManagerConfigurations : IEntityTypeConfiguration<Manager>
	{
		public void Configure(EntityTypeBuilder<Manager> builder)
		{
			builder.ToTable("Managers");

			builder.HasKey(m => m.Id);

			builder.Property(m => m.Name)
				.IsRequired()
				.HasMaxLength(200);

			builder.Property(m => m.Role)
				.IsRequired()
				.HasMaxLength(100);

			builder.Property(m => m.Email)
				.IsRequired()
				.HasMaxLength(150);

			builder.Property(m => m.Password)
				.IsRequired();

			builder.HasOne(m => m.Company)
				.WithOne(c => c.Manager)
				.HasForeignKey<Company>(m => m.ManagerId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
