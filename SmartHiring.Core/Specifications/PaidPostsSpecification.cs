using SmartHiring.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SmartHiring.Core.Specifications
{
    public class PaidPostsSpecification : BaseSpecifications<Post>
    {
        public PaidPostsSpecification()
        : base(p => p.PaymentStatus == PaymentStatus.Paid)
        {
        }

        public PaidPostsSpecification(int companyId)
            : base(p => p.CompanyId == companyId && p.PaymentStatus == PaymentStatus.Paid)
        {
        }
    }
}
