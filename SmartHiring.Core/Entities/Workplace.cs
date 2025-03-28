namespace SmartHiring.Core.Entities
{
	public class Workplace :BaseEntity
	{
		public string WorkplaceType { get; set; }
		public ICollection<PostWorkplace> PostWorkplaces { get; set; } = new HashSet<PostWorkplace>();
	}
}
