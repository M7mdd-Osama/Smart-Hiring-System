using SmartHiring.APIs.Helpers;
using System.ComponentModel.DataAnnotations;

namespace SmartHiring.APIs.DTOs
{
	public class BaseUserDto
	{
		[Required(ErrorMessage = "Email is required")]
		[EmailAddress(ErrorMessage = "Invalid email format")]
		[BusinessDomainEmail(ErrorMessage = "Please use a business domain Email")]
		public string Email { get; set; }

		[Required(ErrorMessage = "Password is required")]
		public string Password { get; set; }

		[Required(ErrorMessage = "Confirm password is required")]
		[Compare("Password", ErrorMessage = "Passwords do not match")]
		public string RePassword { get; set; }

		[Required(ErrorMessage = "Phone number is required")]
		[RegularExpression(@"^\+?[1-9]\d{1,14}$", ErrorMessage = "Invalid phone number format")]
		public string PhoneNumber { get; set; }
	}
}
