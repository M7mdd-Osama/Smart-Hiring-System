using System.Linq.Expressions;
using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
    public class PostsWithApplicationsSpec : BaseSpec<Post>
    {
        public PostsWithApplicationsSpec(Expression<Func<Post, bool>> criteria)
            : base(criteria)
        {
            AddInclude(p => p.Applications);
        }
        public PostsWithApplicationsSpec(int companyId)
            : this(p => p.CompanyId == companyId)
        {
        }
        public PostsWithApplicationsSpec(int companyId, DateTime fromDate, DateTime toDate)
            : this(p => p.CompanyId == companyId &&
                        p.PostDate >= fromDate &&
                        p.PostDate <= toDate)
        {
        }
        public PostsWithApplicationsSpec(List<int> postIds)
            : this(p => postIds.Contains(p.Id))
        {
        }
        public PostsWithApplicationsSpec(List<int> postIds, DateTime fromDate, DateTime toDate)
            : this(p => postIds.Contains(p.Id) &&
                        p.PostDate >= fromDate &&
                        p.PostDate <= toDate)
        {
        }
    }
}