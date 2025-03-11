using SmartHiring.APIs.DTOs;
using System.ComponentModel.DataAnnotations;

public class RegisterDto : BaseUserDto
{
	[Required(ErrorMessage = "First name is required")]
	public string FirstName { get; set; }

	[Required(ErrorMessage = "Last name is required")]
	public string LastName { get; set; }

	[Required(ErrorMessage = "Role is required")]
	[RegularExpression(@"^(HR|Manager|Agency)$", ErrorMessage = "Invalid role")]
	public string Role { get; set; }

	public string? CompanyEmail { get; set; }
	public string? CompanyPassword { get; set; }

	public string? AgencyName { get; set; }
}