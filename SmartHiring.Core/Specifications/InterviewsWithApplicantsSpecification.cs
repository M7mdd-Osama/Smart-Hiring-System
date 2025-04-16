using SmartHiring.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHiring.Core.Specifications
{
    public class InterviewsWithApplicantsSpecification : BaseSpecifications<Interview>
    {
        public InterviewsWithApplicantsSpecification(DateTime fromDate, DateTime toDate, int companyId)
        : base(i =>
            i.Post.CompanyId == companyId &&
            i.Date >= fromDate &&
            i.Date <= toDate)
        {
            AddInclude(i => i.Applicant);
            AddInclude(i => i.Post);
        }
    }
}
