namespace SmartHiring.APIs.DTOs
{
    public class EditUserDto
    {
        public string? FirstName { get; set; } 
        public string? LastName { get; set; }  
        public string? AgencyName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? NewPassword { get; set; }
        public AddressDto? Address { get; set; }
        public IFormFile? CompanyLogo { get; set; }
    }
}
