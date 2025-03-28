using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartHiring.Core.Entities;
using SmartHiring.Core.Entities.Identity;
using System.Reflection;

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

		public DbSet<JobType> JobTypes { get; set; }
		public DbSet<ApplicantSkill> ApplicantSkills { get; set; }
		public DbSet<Workplace> Workplaces { get; set; }
		public DbSet<CareerLevel> CareerLevels { get; set; }
		public DbSet<Skill> Skills { get; set; }
		public DbSet<JobCategory> JobCategories { get; set; }

		public DbSet<PostJobType> PostJobTypes { get; set; }
		public DbSet<PostWorkplace> PostWorkplaces { get; set; }
		public DbSet<PostCareerLevel> PostCareerLevels { get; set; }
		public DbSet<PostSkill> PostSkills { get; set; }
		public DbSet<PostJobCategory> PostJobCategories { get; set; }
		public DbSet<SavedPost> SavedPosts { get; set; }
	}
}