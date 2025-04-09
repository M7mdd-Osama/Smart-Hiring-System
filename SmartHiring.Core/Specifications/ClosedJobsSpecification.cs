using SmartHiring.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHiring.Core.Specifications
{
    public class ClosedJobsSpecification : BaseSpecifications<Post>
    {
        public ClosedJobsSpecification(int companyId)
        : base(p => p.CompanyId == companyId && p.JobStatus == JobStatus.Closed)
        {
        }
    }
}
