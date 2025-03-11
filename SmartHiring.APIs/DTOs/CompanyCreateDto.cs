using SmartHiring.Core.Entities;
namespace SmartHiring.APIs.DTOs
{
    public class CompanyCreateDto
    {
        public int Id { get; set; } 
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Industry { get; set; } = string.Empty;
        public string BusinessEmail { get; set; } = string.Empty;
    }
}
