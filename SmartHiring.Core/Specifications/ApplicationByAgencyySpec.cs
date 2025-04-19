using SmartHiring.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHiring.Core.Specifications
{
    public class ApplicationByAgencyySpec : BaseSpec<Application>
    {

            public ApplicationByAgencyySpec(string agencyId, DateTime fromDate, DateTime toDate)
                : base(a =>
                    a.AgencyId == agencyId &&
                    a.ApplicationDate >= fromDate.Date &&
                    a.ApplicationDate < toDate.Date.AddDays(1)) // يشمل كل اليوم الأخير
            {
                AddInclude(a => a.Agency);
            }


    }
}
