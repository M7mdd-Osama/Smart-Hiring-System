using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using SmartHiring.APIs.Settings;
using SmartHiring.Core.Entities;
//using System.Net.Mail;

namespace SmartHiring.APIs.Helpers
{
	public class EmailSettings : ImailSettings
	{
		private MailSettings _options;
		public EmailSettings(IOptions<MailSettings> options)
		{
			_options = options.Value;
		}
		public void SendMail(Email email)
		{
			var mail = new MimeMessage
			{
				Sender = MailboxAddress.Parse(_options.Email),
				Subject = email.Subject
			};
			mail.To.Add(MailboxAddress.Parse(email.To));
			mail.From.Add(new MailboxAddress(_options.DisplayName, _options.Email));

			var builder = new BodyBuilder
			{
				HtmlBody = $@"
            <h2>Welcome to Smart Hiring!</h2>
            <p>{email.Body}</p>
            <p style='color:gray;'>This OTP is valid for 10 minutes only.</p> "
			};
			mail.Body = builder.ToMessageBody();

			using var smtp = new SmtpClient();

			smtp.Connect(_options.Host, _options.Port, SecureSocketOptions.StartTls);

			smtp.Authenticate(_options.Email, _options.Password);
			smtp.Send(mail);

			smtp.Disconnect(true);
		}
	}
}
