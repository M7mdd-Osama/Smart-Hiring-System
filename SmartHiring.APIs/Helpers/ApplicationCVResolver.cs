using AutoMapper;
using SmartHiring.APIs.DTOs;
using SmartHiring.Core.Entities;

namespace SmartHiring.APIs.Helpers
{
    public class ApplicationCVResolver : IValueResolver<Application, PendingCandidateListApplicantDto, string>
    {
        private readonly IConfiguration _configuration;

        public ApplicationCVResolver(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Resolve(Application source, PendingCandidateListApplicantDto destination, string destMember, ResolutionContext context)
        {
            string cvLink = source.CV_Link;

            if (!string.IsNullOrEmpty(cvLink))
                return $"{_configuration["ApiBaseUrl"]}{cvLink}";
            return string.Empty;
        }
    }
}
