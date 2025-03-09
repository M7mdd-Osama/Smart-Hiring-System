using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHiring.Core.Entities
{
	public class JobType : BaseEntity
	{
		public string TypeName { get; set; }
		public ICollection<PostJobType> PostJobTypes { get; set; } = new HashSet<PostJobType>();
	}
}
