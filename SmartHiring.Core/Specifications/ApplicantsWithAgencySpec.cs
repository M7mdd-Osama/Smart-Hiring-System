using SmartHiring.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SmartHiring.Core.Specifications
{
    public class ApplicantsWithAgencySpec : BaseSpec<Application>
    {
        public ApplicantsWithAgencySpec(string hrId, int? companyId, DateTime? fromDate, DateTime? toDate)
            : base(app =>
                (!fromDate.HasValue || app.ApplicationDate >= fromDate.Value) &&
                (!toDate.HasValue || app.ApplicationDate <= toDate.Value) &&
                (string.IsNullOrEmpty(hrId) || app.Post.HRId == hrId) &&
                (!companyId.HasValue || app.Post.CompanyId == companyId.Value))
        {
            AddInclude(app => app.Applicant);
            AddInclude(app => app.Agency);
            AddInclude(app => app.Post);
        }
    }




}
