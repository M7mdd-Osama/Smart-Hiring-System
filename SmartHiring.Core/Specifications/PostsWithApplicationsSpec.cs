using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
    public class PostsWithApplicationsSpec : BaseSpec<Post>
    {
        public PostsWithApplicationsSpec(int companyId)
            : base(p => p.CompanyId == companyId)
        {
            AddInclude(p => p.Applications);
        }

        public PostsWithApplicationsSpec(int companyId, DateTime fromDate, DateTime toDate)
            : base(p => p.CompanyId == companyId &&
                        p.PostDate >= fromDate &&
                        p.PostDate <= toDate)
        {
            AddInclude(p => p.Applications);
        }

        public PostsWithApplicationsSpec(List<int> postIds)
            : base(p => postIds.Contains(p.Id))
        {
            AddInclude(p => p.Applications);
        }

        public PostsWithApplicationsSpec(List<int> postIds, DateTime fromDate, DateTime toDate)
            : base(p => postIds.Contains(p.Id) &&
                        p.PostDate >= fromDate &&
                        p.PostDate <= toDate)
        {
            AddInclude(p => p.Applications);
        }


    }


}
