namespace SmartHiring.Core.Entities
{
	public class JobType : BaseEntity
	{
		public string TypeName { get; set; }
		public ICollection<PostJobType> PostJobTypes { get; set; } = new HashSet<PostJobType>();
	}
}
