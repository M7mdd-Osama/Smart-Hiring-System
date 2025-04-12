using SmartHiring.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHiring.Core.Specifications
{
    public class PendingInterviewsSpecification : BaseSpecifications<Interview>
    {
        public PendingInterviewsSpecification(DateTime fromDate, DateTime toDate, int companyId)
        : base(i =>
            i.Date >= fromDate &&
            i.Date <= toDate &&
            i.InterviewStatus == InterviewStatus.Pending &&
            i.Post.CompanyId == companyId)
        {
            AddInclude(i => i.Post);
            AddInclude(i => i.Applicant);
        }
    }
}
