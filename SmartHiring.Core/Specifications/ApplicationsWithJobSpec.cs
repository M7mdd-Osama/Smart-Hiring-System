using SmartHiring.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHiring.Core.Specifications
{
    public class ApplicationsWithJobSpec : BaseSpecifications<Application>
    {
        public ApplicationsWithJobSpec(int companyId)
            : base(a => a.Post.CompanyId == companyId)
        {
            AddInclude(a => a.Applicant); // دي اللي بتخلي AutoMapper يقدر يجيب بيانات الـ Candidate
            AddInclude(a => a.Post);
        }
    }

}
