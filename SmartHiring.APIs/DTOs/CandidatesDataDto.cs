namespace SmartHiring.APIs.DTOs
{
    public class CandidatesDataDto
    {
        public List<string> MostJobTitle { get; set; } = new();
        public List<string> LeastJobTitle { get; set; } = new();
    }
}
