using System.Linq.Expressions;

namespace SmartHiring.Core.Specifications
{
    public class BaseSpecifications<T> : ISpecifications<T> where T : class
    {
		public Expression<Func<T, bool>> Criteria { get; set; }
		public List<Expression<Func<T, object>>> Includes { get; set; } = new List<Expression<Func<T, object>>>();
		public List<string> IncludeStrings { get; set; } = new List<string>();
		public Expression<Func<T, object>> OrderBy { get; set; }
		public Expression<Func<T, object>> OrderByDesc { get; set; }
		public int Take { get; set; }
		public int Skip { get; set; }
		public bool IsPaginationEnabled { get; set; }

		public BaseSpecifications() { }

		public BaseSpecifications(Expression<Func<T, bool>> criteriaExpression)
		{
			Criteria = criteriaExpression;
		}
		public void AddInclude(Expression<Func<T, object>> includeExpression)
		{
			Includes.Add(includeExpression);
		}
		public void AddIncludeString(string includeString)
		{
			IncludeStrings.Add(includeString);
		}

		public void AddOrderBy(Expression<Func<T, object>> OrderByExpression)
		{
			OrderBy = OrderByExpression;
		}
		public void AddOrderByDesc(Expression<Func<T, object>> OrderByDescExpression)
		{
			OrderByDesc = OrderByDescExpression;
		}

		public void ApplyPagination(int skip, int take)
		{
			IsPaginationEnabled = true;
			Skip = skip;
			Take = take;
		}

	}
}