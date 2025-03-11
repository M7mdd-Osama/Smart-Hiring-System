using SmartHiring.Core.Entities;
namespace SmartHiring.APIs.DTOs
{
    public class CompanyToReturnDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Industry { get; set; }
        public string BusinessEmail { get; set; }
    }
}
