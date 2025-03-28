namespace SmartHiring.Core.Entities
{
	public class CareerLevel : BaseEntity
	{
		public string LevelName { get; set; }
		public ICollection<PostCareerLevel> PostCareerLevels { get; set; } = new HashSet<PostCareerLevel>();
	}
}
