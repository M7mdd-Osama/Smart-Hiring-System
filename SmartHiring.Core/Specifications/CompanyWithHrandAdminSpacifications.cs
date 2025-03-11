using SmartHiring.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHiring.Core.Specifications
{
    public class CompanyWithHrandAdminSpacifications : BaseSpecifications<Company>
    {
        public CompanyWithHrandAdminSpacifications():base()
        {
            Includes.Add(c => c.Manager);
        }
        public CompanyWithHrandAdminSpacifications(int id) : base(c => c.Id == id)
        {
            Includes.Add(c => c.Manager);
        }

    }
}
