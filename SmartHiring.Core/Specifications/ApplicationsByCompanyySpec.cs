using SmartHiring.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHiring.Core.Specifications
{
    public class ApplicationsByCompanyySpec : BaseSpec<Application>
    {
        public ApplicationsByCompanyySpec(int companyId, DateTime fromDate, DateTime toDate)
            : base(a =>
                a.Post.CompanyId == companyId &&
                a.ApplicationDate.Date >= fromDate.Date &&
                a.ApplicationDate.Date <= toDate.Date)
        {
            AddInclude(a => a.Post);
            AddInclude(a => a.Post.Company);
        }
    }
}
