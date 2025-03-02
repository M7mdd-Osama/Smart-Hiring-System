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

            Includes.Add(application => application.Applicant); 
            Includes.Add(application => application.Post);      
            Includes.Add(application => application.Post.HR);   
            Includes.Add(application => application.Agency); 
        }
    }
}
