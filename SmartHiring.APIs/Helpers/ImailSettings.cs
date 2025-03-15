using SmartHiring.Core.Entities;

namespace SmartHiring.APIs.Helpers
{
	public interface ImailSettings
	{
		public Task SendMail(Email email);
	}
}
