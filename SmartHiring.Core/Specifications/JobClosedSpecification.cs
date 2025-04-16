using SmartHiring.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHiring.Core.Specifications
{
    public class JobClosedSpecification : BaseSpecifications<Post>
    {
        public JobClosedSpecification(int companyId)
        : base(p => p.CompanyId == companyId && p.JobStatus == "Closed")  // هنا نستخدم "Closed" بدل الـ enum
        {
        }
    }
}
