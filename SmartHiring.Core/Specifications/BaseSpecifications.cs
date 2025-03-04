using SmartHiring.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SmartHiring.Core.Specifications
{
    public class BaseSpecifications<T> : ISpecifications<T> where T : BaseEntity
    {
        public Expression<Func<T, bool>> Criteria { get; set; }
        public List<Expression<Func<T, object>>> Includes { get; set; } = new List<Expression<Func<T, object>>>();

		public BaseSpecifications() { }

		public BaseSpecifications(Expression<Func<T, bool>> criteriaExpression)
		{
			Criteria = criteriaExpression;
		}

		protected void AddInclude(Expression<Func<T, object>> includeExpression)
		{
			Includes.Add(includeExpression);
		}
	}
}