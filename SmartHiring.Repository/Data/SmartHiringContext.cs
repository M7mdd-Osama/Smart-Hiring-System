using Microsoft.EntityFrameworkCore;
using SmartHiring.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SmartHiring.Repository.Data
{
	public class SmartHiringContext : DbContext
	{
		public SmartHiringContext(DbContextOptions<SmartHiringContext> options) : base(options)
		{
			
		}
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
			base.OnModelCreating(modelBuilder);
		}

		public DbSet<Admin> Admins { get; set; }
		public DbSet<Applicant> Applicants { get; set; }
		public DbSet<HR> HRs { get; set; }
		public DbSet<Agency> Agencies { get; set; }
		public DbSet<Company> Companies { get; set; }
		public DbSet<Manager> Managers { get; set; }
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
