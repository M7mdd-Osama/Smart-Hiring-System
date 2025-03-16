using Microsoft.EntityFrameworkCore;
using SmartHiring.Core.Entities;
using SmartHiring.Core.Specifications;

namespace SmartHiring.Repository
{
	public static class SpecificationEvaluator<T> where T : BaseEntity
	{
		public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecifications<T> Spec)
		{
			var Query = inputQuery;

			if (Spec.Criteria is not null)
			{
				Query = Query.Where(Spec.Criteria);
			}
			 
			if(Spec.OrderBy is not null)
			{
				Query = Query.OrderBy(Spec.OrderBy);
			}
			if (Spec.OrderByDesc is not null)
			{
				Query = Query.OrderByDescending(Spec.OrderByDesc);
			}

			if (Spec.IsPaginationEnabled)
			{
				Query = Query.Skip(Spec.Skip).Take(Spec.Take);
			}

			Query = Spec.Includes.Aggregate(Query, (CurrentQuery, IncludeExpression) =>
				CurrentQuery.Include(IncludeExpression));

			Query = Spec.IncludeStrings.Aggregate(Query, (CurrentQuery, IncludeString) =>
				CurrentQuery.Include(IncludeString));

			return Query;
		}
	}
}
