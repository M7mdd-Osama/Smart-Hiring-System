using SmartHiring.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHiring.Core.Specifications
{
    public class AgenciesByCompanySpecification : BaseSpecifications<Application>
    {
        public AgenciesByCompanySpecification(int companyId)
        : base(a => a.Post.CompanyId == companyId && a.AgencyId != null)
        {
            AddInclude(a => a.Post);
        }
    }
}
