using SmartHiring.Core.Entities;
using System.Security.Cryptography;

namespace SmartHiring.APIs.Helpers
{
	public static class AuthHelper
	{
		public static string GenerateOTP()
		{
			var bytes = new byte[4];
			using (var rng = RandomNumberGenerator.Create())
			{
				rng.GetBytes(bytes);
			}

			int otp = Math.Abs(BitConverter.ToInt32(bytes, 0)) % 1000000;
			return otp.ToString("D6");
		}

		public static async Task SendConfirmationEmail(ImailSettings mailSettings, string email, string otp)
		{
			var emailMessage = new Email
			{
				To = email,
				Subject = "Email Confirmation - Smart Hiring System",
				Body = $"Your OTP for email confirmation is: {otp}"
			};

			await mailSettings.SendMail(emailMessage,true);
		}
	}
}