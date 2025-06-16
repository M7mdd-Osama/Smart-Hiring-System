using System.ComponentModel.DataAnnotations;

namespace SmartHiring.APIs.DTOs
{
    public class CandidateListRequestDto
    {
        [Required(ErrorMessage = "MinimumScore is required.")]
        [Range(0, 100, ErrorMessage = "Minimum score must be between 0 and 100.")]
        public double? MinimumScore { get; set; }
    }
}