using AutoMapper;
namespace SmartHiring.APIs.Helpers
{
    public class CandidateApplicationCVResolver<TSource, TDestination> : IValueResolver<TSource, TDestination, string> where TSource : class
    {
        private readonly IConfiguration _configuration;

        public CandidateApplicationCVResolver(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Resolve(TSource source, TDestination destination, string destMember, ResolutionContext context)
        {
            var sourceType = source.GetType();

            var applicantProp = sourceType.GetProperty("Applicant");
            var candidateListProp = sourceType.GetProperty("CandidateList");

            if (applicantProp == null || candidateListProp == null)
                return string.Empty;

            var applicant = applicantProp.GetValue(source);
            var candidateList = candidateListProp.GetValue(source);

            if (applicant == null || candidateList == null)
                return string.Empty;

            var applicantType = applicant.GetType();
            var candidateListType = candidateList.GetType();

            var applicationsProp = applicantType.GetProperty("Applications");
            var postIdProp = candidateListType.GetProperty("PostId");

            if (applicationsProp == null || postIdProp == null)
                return string.Empty;

            var applications = applicationsProp.GetValue(applicant) as System.Collections.IEnumerable;
            var postId = postIdProp.GetValue(candidateList);

            if (applications == null || postId == null)
                return string.Empty;

            foreach (var app in applications)
            {
                var appType = app.GetType();
                var appPostIdProp = appType.GetProperty("PostId");
                var appCVLinkProp = appType.GetProperty("CV_Link");

                if (appPostIdProp == null || appCVLinkProp == null)
                    continue;

                var appPostId = appPostIdProp.GetValue(app);

                if (appPostId != null && appPostId.Equals(postId))
                {
                    var cvLink = appCVLinkProp.GetValue(app) as string;
                    if (!string.IsNullOrEmpty(cvLink))
                    {
                        return $"{_configuration["ApiBaseUrl"]}{cvLink}";
                    }
                    return string.Empty;
                }
            }

            return string.Empty;
        }
    }
}