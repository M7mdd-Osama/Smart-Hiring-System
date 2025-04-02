using SmartHiring.Core.Entities;

namespace SmartHiring.APIs.Helpers
{
	public interface ImailSettings
	{
		Task SendMail(Email email, bool includeOtpMessage);
	}
}