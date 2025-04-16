using SmartHiring.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHiring.Core.Specifications
{
    public class ApplicationsByAgencySpecification : BaseSpecifications<Application>
    {
        public ApplicationsByAgencySpecification(string agencyUserId)
        : base(a => a.AgencyId == agencyUserId)
        {
            AddInclude(a => a.Post);
        }
    }
}
