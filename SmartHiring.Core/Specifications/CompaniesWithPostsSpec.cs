using SmartHiring.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHiring.Core.Specifications
{

    public class CompaniesWithPostsSpec : BaseSpec<Company>
    {
        public CompaniesWithPostsSpec(int? companyId)
            : base(c => companyId == null || c.Id == companyId)
        {
            AddInclude(c => c.Posts);
        }
    }


}
