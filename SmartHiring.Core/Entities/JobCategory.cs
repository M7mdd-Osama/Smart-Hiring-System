namespace SmartHiring.Core.Entities
{
	public class JobCategory : BaseEntity
	{
		public string Name { get; set; }
		public ICollection<PostJobCategory> PostJobCategories { get; set; } = new HashSet<PostJobCategory>();
	}
}