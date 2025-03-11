using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHiring.Core.Entities
{
	public class CareerLevel : BaseEntity
	{
		public string LevelName { get; set; }
		public ICollection<PostCareerLevel> PostCareerLevels { get; set; } = new HashSet<PostCareerLevel>();
	}
}
