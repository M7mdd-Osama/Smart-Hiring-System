using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
    public class ApplicationsWithAgencyySpec : BaseSpec<Application>
    {
        public ApplicationsWithAgencyySpec()
        {
            AddInclude(a => a.Agency);
        }
        public ApplicationsWithAgencyySpec(int companyId)
            : base(a => a.Post.CompanyId == companyId)
        {
            AddInclude(a => a.Agency);
            AddInclude(a => a.Post);
        }
    }
}
