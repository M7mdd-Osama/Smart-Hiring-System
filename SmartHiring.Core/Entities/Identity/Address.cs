using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHiring.Core.Entities.Identity
{
	public class Address
	{
		public int Id { get; set; }
		public string City { get; set; }
		public string Country { get; set; }
		public string AppUserId { get; set; } // FK
		[ForeignKey("AppUserId")]
		public AppUser User { get; set; }
	}
}
