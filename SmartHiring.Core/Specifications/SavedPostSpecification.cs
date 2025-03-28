using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
	public class SavedPostSpecification : BaseSpecifications<SavedPost>
	{
		public SavedPostSpecification(string userId)
		: base(s => s.UserId == userId)
		{
		}
	}
}