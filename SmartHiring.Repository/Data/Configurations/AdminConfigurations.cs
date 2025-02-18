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
	public class AdminConfigurations : IEntityTypeConfiguration<Admin>
	{
		public void Configure(EntityTypeBuilder<Admin> builder)
		{
			builder.ToTable("Admins");

			builder.HasKey(a => a.Id);

			builder.Property(a => a.Name)
				.IsRequired()
				.HasMaxLength(100);

			builder.Property(a => a.Email)
				.IsRequired()
				.HasMaxLength(150);

			builder.Property(a => a.Password)
				.IsRequired()
				.HasMaxLength(255);
		}
	}
}
