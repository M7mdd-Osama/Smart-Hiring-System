namespace SmartHiring.APIs.DTOs
{
    public class TopAgencyDto
    {
        public int TotalAgencies { get; set; }
        public List<TopAgencyItemDto> TopAgencies { get; set; } = new();

    }
}
