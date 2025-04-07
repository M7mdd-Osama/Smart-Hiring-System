namespace SmartHiring.APIs.DTOs
{
    public class UpdateCompanyByAdminDto
    {
        public string? Name { get; set; }
        public string? BusinessEmail { get; set; }
        public string? Password { get; set; }
        public string? Phone { get; set; }
        public IFormFile? Logo { get; set; }
    }
}
