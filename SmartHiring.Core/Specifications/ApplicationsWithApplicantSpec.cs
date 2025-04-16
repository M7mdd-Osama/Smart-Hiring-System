using SmartHiring.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHiring.Core.Specifications
{
    public class ApplicationsWithApplicantSpec : BaseSpecifications<Application>
    {
        public ApplicationsWithApplicantSpec(int jobId)
            : base(a => a.PostId == jobId)
        {
            AddInclude(a => a.Applicant);
            AddInclude(a => a.Post);
        }
    }
}
