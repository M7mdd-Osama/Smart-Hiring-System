using SmartHiring.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHiring.Core.Specifications
{
    public class JobsWithInterviewsSpec : BaseSpecifications<Post>
    {
        public JobsWithInterviewsSpec(int companyId)
            : base(j => j.Company.Id == companyId)
        {
            AddInclude(j => j.Interviews);
        }

    }
}
