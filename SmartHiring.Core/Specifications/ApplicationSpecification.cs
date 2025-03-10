using SmartHiring.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SmartHiring.Core.Specifications
{
    public class ApplicationSpecification : BaseSpecifications<Application>
    {
        // 🔥 Constructor جديد لجلب جميع الطلبات
        public ApplicationSpecification() : base(application => true)
        {
            AddInclude(application => application.Applicant);
            AddInclude(application => application.Post);
            AddInclude(application => application.Post.HR);
            AddInclude(application => application.Agency);
        }

        public ApplicationSpecification(int applicationId)
            : base(application => application.Id == applicationId)
        {
            AddInclude(application => application.Applicant);
            AddInclude(application => application.Post);
            AddInclude(application => application.Post.HR);
            AddInclude(application => application.Agency);
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
