using SmartHiring.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHiring.Core.Specifications
{
    public class AllCompaniesSpecification : BaseSpecifications<Company>
    {
        public AllCompaniesSpecification()
        {
            // مفيش فلترة هنا، دي مجرد GetAll
        }
    }
}
