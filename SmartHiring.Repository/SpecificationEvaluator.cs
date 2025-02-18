using Microsoft.EntityFrameworkCore;
using SmartHiring.Core.Entities;
using SmartHiring.Core.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHiring.Repository
{
	public static class SpecificationEvaluator<T> where T : BaseEntity
	{
		public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecifications<T> Spec) 
		{
			var Query = inputQuery;
			if(Spec.Criteria is not null)
			{
				Query = Query.Where(Spec.Criteria);
			}
			Query = Spec.Includes.Aggregate(Query,(CurrentQuery,IncludeExpression) => CurrentQuery.Include(IncludeExpression));
			return Query;
		}
	}
}
