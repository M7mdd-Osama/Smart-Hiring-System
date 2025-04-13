using SmartHiring.Core.Entities.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartHiring.Core.Entities
{
    public class Post : BaseEntity
	{
		public string JobTitle { get; set; }
		public string Description { get; set; }
		public string Requirements { get; set; }
		public DateTime PostDate { get; set; }
		public DateTime Deadline { get; set; }
        public string JobStatus { get; set; } = "Open";
		public decimal MinSalary { get; set; }
		public decimal MaxSalary { get; set; }
		public string Currency { get; set; } // EGP, USD, etc.  
		public int MinExperience { get; set; }
		public int MaxExperience { get; set; }
		public string PaymentStatus { get; set; } = "Pending Payment";
		public string Country { get; set; }
		public string City { get; set; }
		public string HRId { get; set; }
		[ForeignKey("HRId")]
		public AppUser HR { get; set; }
		public int CompanyId { get; set; }
		[ForeignKey("CompanyId")]
		public Company Company { get; set; }

		public ICollection<PostJobCategory> PostJobCategories { get; set; } = new HashSet<PostJobCategory>();
		public ICollection<PostJobType> PostJobTypes { get; set; } = new HashSet<PostJobType>();
		public ICollection<PostWorkplace> PostWorkplaces { get; set; } = new HashSet<PostWorkplace>();
		public ICollection<PostSkill> PostSkills { get; set; } = new HashSet<PostSkill>();
		public ICollection<PostCareerLevel> PostCareerLevels { get; set; } = new HashSet<PostCareerLevel>();
		public ICollection<Application> Applications { get; set; } = new HashSet<Application>();
		public ICollection<CandidateList> CandidateLists { get; set; } = new HashSet<CandidateList>();
		public ICollection<Interview> Interviews { get; set; } = new HashSet<Interview>();
		public ICollection<SavedPost> SavedPosts { get; set; } = new HashSet<SavedPost>();
		public ICollection<Note> Notes { get; set; } = new HashSet<Note>();

		public string? PaymentIntentId { get; set; }
		public string? ClientSecret { get; set; }
	}
}
