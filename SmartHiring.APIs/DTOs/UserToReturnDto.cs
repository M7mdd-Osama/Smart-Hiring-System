using SmartHiring.Core.Entities;

namespace SmartHiring.APIs.DTOs
{
    public class UserToReturnDto
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public string Industry { get; set; }
        public string BusinessEmail { get; set; }

        public int AdminId { get; set; }
        public string Admin { get; set; }

        public int ManagerId { get; set; }
        public string Manager { get; set; }
    }
}
