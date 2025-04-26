using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
    public class ApplicationsWithPostAndAgencySpec : BaseSpec<Application>
    {
        public ApplicationsWithPostAndAgencySpec(int? companyId, DateTime fromDate, DateTime toDate)
            : base(a =>
                a.Post.PostDate >= fromDate &&
                a.Post.PostDate <= toDate &&
                (companyId == null || a.Post.CompanyId == companyId))
        {
            AddInclude(a => a.Agency);
            AddInclude(a => a.Post);
        }
    }

}
