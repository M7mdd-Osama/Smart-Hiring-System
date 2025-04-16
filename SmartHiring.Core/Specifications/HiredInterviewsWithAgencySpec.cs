using SmartHiring.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHiring.Core.Specifications
{
    public class HiredInterviewsWithHRSpec : BaseSpecifications<Interview>
    {
        public HiredInterviewsWithHRSpec(int? companyId = null)
            : base(i => i.InterviewStatus == InterviewStatus.Hired &&
                        (companyId == null || i.Post.Company.Id == companyId))
        {
            AddInclude(i => i.HR);
            AddInclude(i => i.Post);
            AddInclude(i => i.Post.Company);
        }
    }
}
