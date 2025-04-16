using SmartHiring.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHiring.Core.Specifications
{
    public class InterviewsForJobSpec : BaseSpecifications<Interview>
    {
        public InterviewsForJobSpec(int jobId)
            : base(i => i.PostId == jobId)
        {
            AddInclude(i => i.Post);
            AddInclude(i => i.Applicant);

        }
    }
}
