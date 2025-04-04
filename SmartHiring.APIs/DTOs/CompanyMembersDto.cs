namespace SmartHiring.APIs.DTOs
{
    public class CompanyMembersDto
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string BusinessEmail { get; set; }
        public string LogoUrl { get; set; }

        public MembersInfoDto HR { get; set; }
        public MembersInfoDto Manager { get; set; }
    }
}
