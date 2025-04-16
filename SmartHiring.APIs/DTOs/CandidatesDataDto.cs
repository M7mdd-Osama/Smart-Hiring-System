using Newtonsoft.Json;

namespace SmartHiring.APIs.DTOs
{
    public class CandidatesDataDto
    {
        public List<string> MostJobCandidates { get; set; }

        public List<string> LeastJobCandidates { get; set; }
    }
}
