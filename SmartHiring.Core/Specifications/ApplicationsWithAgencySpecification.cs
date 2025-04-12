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
            : base(a => a.AgencyId != null)
        {
        }

        // للـ HR/Manager - Applications على الشركة بتاعتهم، برضو لازم تكون من Agencies
        public ApplicationsWithAgencySpecification(int companyId)
            : base(a => a.AgencyId != null && a.Post.CompanyId == companyId)
        {
            AddInclude(a => a.Post); // ضروري عشان نقدر نوصل لـ Post.CompanyId
        }
    }
}
