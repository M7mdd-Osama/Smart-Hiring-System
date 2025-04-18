namespace SmartHiring.APIs.DTOs
{
    public class ApplicationDetailDto
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } // فعليًا هو ApplicationDate
        public string FormattedDate { get; set; }
    }
}
