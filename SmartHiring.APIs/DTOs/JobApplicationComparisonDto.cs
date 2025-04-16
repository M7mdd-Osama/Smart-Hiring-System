using Newtonsoft.Json;

namespace SmartHiring.APIs.DTOs
{
    public class JobApplicationComparisonDto
    {
        public int TotalJobsApplied { get; set; }

        public int MostJobCount { get; set; }

        public int LeastJobCount { get; set; }

        public CandidatesDataDto CandidatesData { get; set; }
    }
}
