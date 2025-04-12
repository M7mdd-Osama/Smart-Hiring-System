using SmartHiring.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHiring.Core.Specifications
{
    public class PaidJobsByCompanySpecification : BaseSpecifications<Post>
    {
        public PaidJobsByCompanySpecification(int companyId)
            : base(p => p.CompanyId == companyId && p.PaymentStatus == "Paid")
        {
        }

        // لو حابب تجيب حاجات مرتبطة زي الشركة
        public PaidJobsByCompanySpecification()
            : base(p => p.PaymentStatus == "Paid")
        {
        }
    }
}
