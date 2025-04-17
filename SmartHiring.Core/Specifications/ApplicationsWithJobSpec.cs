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
    }

}
