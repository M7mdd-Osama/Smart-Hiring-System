using System.ComponentModel.DataAnnotations;

namespace SmartHiring.APIs.DTOs
{
	public class RegisterCompanyDto : BaseUserDto
	{
		[Required(ErrorMessage = "Company name is required")]
		public string CompanyName { get; set; }

		public IFormFile? CompanyLogoUrl { get; set; }
	}
}
