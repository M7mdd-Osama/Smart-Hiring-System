using SmartHiring.Core.Specifications;
using System.Linq.Expressions;

namespace SmartHiring.Core.Repositories
{
	public interface IGenericRepo<T> where T : class
	{
		#region Without Spec
		Task<IEnumerable<T>> GetAllAsync();
		Task<T> GetByIdAsync(object id);
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate);
        Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate);

        #endregion

        #region With Spec
        Task<IEnumerable<T>> GetAllWithSpecAsync(ISpec<T> Spec);
		Task<T> GetByEntityWithSpecAsync(ISpec<T> Spec);
        Task<int> GetCountWithSpecAsync(ISpec<T> Spec);
        #endregion

		#region ADD/UPDATE/DELETE
		Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        Task UpdateAsync(T entity);
		Task DeleteAsync(T entity);
		Task SaveChangesAsync();

		#endregion
	}
}
