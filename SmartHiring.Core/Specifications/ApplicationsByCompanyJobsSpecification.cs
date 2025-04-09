using SmartHiring.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHiring.Core.Specifications
{
    public class ApplicationsByCompanyJobsSpecification : BaseSpecifications<Application>
    {
        public ApplicationsByCompanyJobsSpecification(int companyId)
        : base(a => a.Post.CompanyId == companyId)
        {
            AddInclude(a => a.Post);
        }
    }
}
