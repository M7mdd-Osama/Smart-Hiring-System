using System.ComponentModel.DataAnnotations.Schema;

namespace SmartHiring.Core.Entities.Identity
{
	public class Address
	{
		public int Id { get; set; }
		public string City { get; set; }
		public string Country { get; set; }
		public string AppUserId { get; set; } 
		[ForeignKey("AppUserId")]
		public AppUser User { get; set; }
	}
}
