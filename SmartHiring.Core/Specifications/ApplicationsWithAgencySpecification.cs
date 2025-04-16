using SmartHiring.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHiring.Core.Specifications
{
    public class ApplicationsWithAgencySpecification : BaseSpecifications<Application>
    {
        // للـ Admin - كل الـ Applications اللي فيها AgencyId مش null
        public ApplicationsWithAgencySpecification()
        {
            AddInclude(a => a.Agency);
        }

        public ApplicationsWithAgencySpecification(int companyId)
            : base(a => a.Post.CompanyId == companyId)
        {
            AddInclude(a => a.Agency);
            AddInclude(a => a.Post);
        }
    }
}
