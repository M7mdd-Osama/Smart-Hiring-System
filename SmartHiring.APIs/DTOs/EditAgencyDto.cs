namespace SmartHiring.APIs.DTOs
{
    public class EditAgencyDto
    {
        public string AgencyName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string CurrentPassword { get; set; }
        public AddressDto Address { get; set; }
    }
}
