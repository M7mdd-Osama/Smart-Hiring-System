namespace SmartHiring.Core.Specifications
{
	public class PostSpecParams
	{
		public string? Sort { get; set; }
		public int? typeId { get; set; }
		public int? careerLevelId { get; set; }
		public int? workplaceId { get; set; }

		private int pageSize = 5;
		public int PageSize
		{
			get { return pageSize; }
			set { pageSize = value > 10 ? 10 : value; }
		}
		public int PageIndex { get; set; } = 1;
	}
}
