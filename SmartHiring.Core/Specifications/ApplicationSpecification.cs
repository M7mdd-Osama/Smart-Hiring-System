using SmartHiring.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SmartHiring.Core.Specifications
{
    public class ApplicationSpecification : BaseSpecifications<Application>
    {
        public ApplicationSpecification(int applicationId)
            : base(application => application.Id == applicationId)
        {
            AddInclude(application => application.Applicant); // ✅ تحميل بيانات المتقدم
            AddInclude(application => application.Post);      // ✅ تحميل تفاصيل الوظيفة
            AddInclude(application => application.Post.HR);   // ✅ تحميل بيانات الـ HR
            AddInclude(application => application.Agency);    // ✅ تحميل بيانات الـ Agency
        }

        public ApplicationSpecification(int jobId, bool byJob)
            : base(application => application.PostId == jobId)
        {
            AddInclude(application => application.Applicant);
            AddInclude(application => application.Post);
            AddInclude(application => application.Post.HR);
            AddInclude(application => application.Agency);
        }
    }
}