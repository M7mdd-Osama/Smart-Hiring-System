using SmartHiring.APIs.DTOs;
using System.ComponentModel.DataAnnotations;

public class RegisterDto : BaseUserDto
{
	public string? FirstName { get; set; }
	public string? LastName { get; set; }

	[Required(ErrorMessage = "Role is required")]
	[RegularExpression(@"^(HR|Manager|Agency)$", ErrorMessage = "Invalid role")]
	public string Role { get; set; }

	public string? CompanyEmail { get; set; }
	public string? CompanyPassword { get; set; }

	public string? AgencyName { get; set; }
}