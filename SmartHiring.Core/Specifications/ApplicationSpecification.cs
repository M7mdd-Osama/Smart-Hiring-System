using SmartHiring.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SmartHiring.Core.Specifications
{
    public class ApplicationSpecification : ISpecifications<Application>
    {
        public Expression<Func<Application, bool>> Criteria { get; set; }
        public List<Expression<Func<Application, object>>> Includes { get; set; } = new List<Expression<Func<Application, object>>>();

        public ApplicationSpecification(int applicationId)
        {
            Criteria = application => application.Id == applicationId;

            Includes.Add(application => application.Applicant); // ✅ تحميل بيانات المتقدم
            Includes.Add(application => application.Post);      // ✅ تحميل تفاصيل الوظيفة
            Includes.Add(application => application.Post.HR);   // ✅ تحميل بيانات الـ HR
            Includes.Add(application => application.Agency);    // ✅ تحميل بيانات الـ Agency
        }
    }
}
