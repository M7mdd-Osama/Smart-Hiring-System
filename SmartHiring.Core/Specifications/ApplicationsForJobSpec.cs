using SmartHiring.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHiring.Core.Specifications
{
    public class ApplicationsForJobSpec : BaseSpecifications<Application>
    {
        public ApplicationsForJobSpec(int jobId)
            : base(app => app.PostId == jobId)
        {
            AddInclude(app => app.Post);
        }
    }
}
