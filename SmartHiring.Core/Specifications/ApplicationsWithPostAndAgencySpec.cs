using SmartHiring.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHiring.Core.Specifications
{

    // Manager فقط
    public class ApplicationsWithPostAndAgencySpec : BaseSpec<Application>
    {
        public ApplicationsWithPostAndAgencySpec(int? companyId, DateTime fromDate, DateTime toDate)
            : base(a =>
                a.Post.PostDate >= fromDate &&
                a.Post.PostDate <= toDate &&
                (companyId == null || a.Post.CompanyId == companyId))
        {
            AddInclude(a => a.Agency);
            AddInclude(a => a.Post);
        }
    }

}
