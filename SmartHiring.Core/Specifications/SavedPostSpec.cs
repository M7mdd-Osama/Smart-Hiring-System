using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
	public class SavedPostSpec : BaseSpec<SavedPost>
	{
		public SavedPostSpec(string userId)
		: base(s => s.UserId == userId)
		{
		}
	}
}