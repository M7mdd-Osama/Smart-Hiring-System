namespace SmartHiring.APIs.DTOs
{
    public class SubmitApplicationDto
    {
        public string FName { get; set; }
        public string LName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public IFormFile CVFile { get; set; }
    }
}
