using SmartHiring.Core.Entities;

namespace SmartHiring.APIs.Helpers
{
	public interface ImailSettings
	{
		Task SendMail(Email email, bool includeOtpMessage);
        Task SendMailWithAttachment(string to, string subject, string htmlBody, byte[] attachment, string attachmentName);
        Task SendMailWithAttachmentAndReplyTo(string to, string subject, string htmlBody, byte[] attachment, string attachmentName, string replyToEmail, string replyToName);
    }
}