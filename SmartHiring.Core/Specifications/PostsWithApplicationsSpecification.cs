using SmartHiring.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHiring.Core.Specifications
{
    public class PostsWithApplicationsSpecification : BaseSpecifications<Post>
    {
        public PostsWithApplicationsSpecification(List<int> postIds)
        : base(p => postIds.Contains(p.Id))
        {
            AddInclude(p => p.Applications); // ← مهم جدًا
        }

        public PostsWithApplicationsSpecification(int companyId)
            : base(p => p.CompanyId == companyId)
        {
            AddInclude(p => p.Applications); // ← مهم جدًا
        }

    }
}
