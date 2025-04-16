using SmartHiring.Core.Entities;
using SmartHiring.Core.Specifications;
using System.Linq.Expressions;

namespace SmartHiring.Core.Repositories
{
	public interface IGenericRepository<T> where T : class
	{
		#region Without Spec
		Task<IEnumerable<T>> GetAllAsync();
		Task<T> GetByIdAsync(object id);
		#endregion

		#region With Spec
		Task<IEnumerable<T>> GetAllWithSpecAsync(ISpecifications<T> Spec);
		Task<T> GetByEntityWithSpecAsync(ISpecifications<T> Spec);
		#endregion

		Task<int> GetCountWithSpecAsync(ISpecifications<T> Spec);
		Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
		Task AddRangeAsync(IEnumerable<T> entities);

		// -----------------------------------------

		Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate);

		Task AddAsync(T entity);
		Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
		Task SaveChangesAsync();
    }
}
