using SmartHiring.Core.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHiring.Core.Specifications
{
    public class AllAgenciesSpecification : BaseSpecifications<AppUser>
    {
        public AllAgenciesSpecification()
           : base(a => a.AgencyName != null) // نفترض أن الـ Agencies عندهم AgencyName
        {
        }
    }
}
