using SmartHiring.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHiring.Core.Specifications
{
    public class InterviewByStatusSpecification : BaseSpecifications<Interview>
    {
        public InterviewByStatusSpecification(int companyId, InterviewStatus status)
        : base(i => i.Post.CompanyId == companyId && i.InterviewStatus == status)
        {
            AddInclude(i => i.Post);
        }
    }
}
