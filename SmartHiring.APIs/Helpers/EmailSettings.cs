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
		private readonly MailSettings _options;

		public EmailSettings(IOptions<MailSettings> options)
		{
			_options = options.Value;
		}

		public async Task SendMail(Email email, bool includeOtpMessage)
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
            <p>{email.Body}</p>"
			};

			if (includeOtpMessage)
			{
				builder.HtmlBody += "<p style='color:gray;'>This OTP is valid for 10 minutes only.</p>";
			}

			mail.Body = builder.ToMessageBody();

			using var smtp = new SmtpClient();

			await smtp.ConnectAsync(_options.Host, _options.Port, SecureSocketOptions.StartTls);
			await smtp.AuthenticateAsync(_options.Email, _options.Password);
			await smtp.SendAsync(mail);
			await smtp.DisconnectAsync(true);
		}
        public async Task SendMailWithAttachment(string to, string subject, string htmlBody, byte[] attachment, string attachmentName)
        {
                var mail = new MimeMessage
                {
                    Sender = MailboxAddress.Parse(_options.Email),
                    Subject = subject
                };
                mail.To.Add(MailboxAddress.Parse(to));
                mail.From.Add(new MailboxAddress(_options.DisplayName, _options.Email));

                var builder = new BodyBuilder
                {
                    HtmlBody = htmlBody
                };

                builder.Attachments.Add(attachmentName, attachment, ContentType.Parse("application/pdf"));
                mail.Body = builder.ToMessageBody();

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(_options.Host, _options.Port, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_options.Email, _options.Password);
                await smtp.SendAsync(mail);
                await smtp.DisconnectAsync(true);
        }
        public async Task SendMailWithAttachmentAndReplyTo(string to, string subject, string htmlBody, byte[] attachment, string attachmentName, string replyToEmail, string replyToName)
        {
            var mail = new MimeMessage
            {
                Sender = MailboxAddress.Parse(_options.Email),
                Subject = subject
            };
            mail.To.Add(MailboxAddress.Parse(to));
            mail.From.Add(new MailboxAddress(_options.DisplayName, _options.Email));

            // إضافة Reply-To
            if (!string.IsNullOrWhiteSpace(replyToEmail))
            {
                mail.ReplyTo.Add(new MailboxAddress(replyToName, replyToEmail));
            }

            var builder = new BodyBuilder
            {
                HtmlBody = htmlBody
            };

            builder.Attachments.Add(attachmentName, attachment, ContentType.Parse("application/pdf"));
            mail.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_options.Host, _options.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_options.Email, _options.Password);
            await smtp.SendAsync(mail);
            await smtp.DisconnectAsync(true);
        }
    }
}
