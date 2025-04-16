using SmartHiring.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHiring.Core.Specifications
{
    public class PostsByCompanySpecification : BaseSpecifications<Post>
    {
        public PostsByCompanySpecification(int companyId)
        {
            Criteria = post => post.CompanyId == companyId;  // Replace with your filtering criteria
        }
    }
}
