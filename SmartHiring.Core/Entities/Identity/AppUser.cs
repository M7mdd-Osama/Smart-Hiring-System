using Microsoft.AspNetCore.Identity;

namespace SmartHiring.Core.Entities.Identity
{
	public class AppUser : IdentityUser
	{
        public string DisplayName => !string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(LastName)
               ? $"{FirstName} {LastName}"
               : AgencyName ?? "N/A";
        public string? FirstName { get; set; }
		public string? LastName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? ConfirmationCode { get; set; }
		public DateTime? ConfirmationCodeExpires { get; set; }
		public string? AgencyName { get; set; }

		public Address Address { get; set; }

		public Company? HRCompany { get; set; } 

		public Company? ManagedCompany { get; set; }

        public ICollection<Application>? Applications { get; set; } = new HashSet<Application>(); 
		public ICollection<AgencyApplicant>? AgencyApplicants { get; set; } = new HashSet<AgencyApplicant>(); 
		public ICollection<Interview>? Interviews { get; set; } = new HashSet<Interview>();
		public ICollection<Post>? Posts { get; set; } = new HashSet<Post>();
		public ICollection<SavedPost> SavedPosts { get; set; } = new HashSet<SavedPost>();
		public ICollection<Note> Notes { get; set; } = new HashSet<Note>();
	}
}