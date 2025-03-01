using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartHiring.Core.Entities;
using SmartHiring.Core.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SmartHiring.Repository.Data
{
	public class SmartHiringDbContext : IdentityDbContext<AppUser>
	{
		public SmartHiringDbContext(DbContextOptions<SmartHiringDbContext> options) : base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
		}

		public DbSet<Applicant> Applicants { get; set; }
		public DbSet<Company> Companies { get; set; }
		public DbSet<Post> Posts { get; set; }
		public DbSet<Application> Applications { get; set; }
		public DbSet<Interview> Interviews { get; set; }
		public DbSet<CandidateList> CandidateLists { get; set; }
		public DbSet<CandidateListApplicant> CandidateListApplicants { get; set; }
		public DbSet<ApplicantPhone> ApplicantPhones { get; set; }
		public DbSet<ApplicantAddress> ApplicantAddresses { get; set; }
		public DbSet<CompanyPhone> CompanyPhones { get; set; }
		public DbSet<AgencyApplicant> AgencyApplicants { get; set; }
	}
}
