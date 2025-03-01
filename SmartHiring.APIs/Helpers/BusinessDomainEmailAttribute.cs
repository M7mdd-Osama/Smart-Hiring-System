using System.ComponentModel.DataAnnotations;

namespace SmartHiring.APIs.Helpers
{
	public class BusinessDomainEmailAttribute : ValidationAttribute
	{
		private readonly List<string> _blockedDomains = new() { "gmail.com", "yahoo.com", "outlook.com", "hotmail.com" };

		protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
		{
			if (value is string email)
			{
				var domain = email.Split('@').Last().ToLower();
				if (_blockedDomains.Contains(domain))
					return new ValidationResult("Please use a business domain Email");
			}

			return ValidationResult.Success;
		}
	}
}