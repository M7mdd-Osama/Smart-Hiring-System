using SmartHiring.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHiring.Core.Specifications
{
    public class JobWithCompanySpec : BaseSpecifications<Post>
    {
        public JobWithCompanySpec(int jobId)
            : base(p => p.Id == jobId)
        {
            AddInclude(p => p.Company);
        }
    }
}
