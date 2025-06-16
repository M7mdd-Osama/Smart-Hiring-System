using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
    public class ApplicationsWithJobSpec : BaseSpec<Application>
    {
        public ApplicationsWithJobSpec(int companyId)
            : base(a => a.Post.CompanyId == companyId)
        {
            AddInclude(a => a.Applicant);
            AddInclude(a => a.Post);
        }

        public ApplicationsWithJobSpec(int companyId, DateTime fromDate, DateTime toDate)
            : base(a => a.Post.CompanyId == companyId &&
                        a.Post.PostDate >= fromDate &&
                        a.Post.PostDate <= toDate &&
                        a.IsShortlisted == true)
        {
            AddInclude(a => a.Applicant);
            AddInclude(a => a.Post);
        }
    }


}
