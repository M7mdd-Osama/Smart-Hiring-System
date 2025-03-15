using SmartHiring.Core.Entities;
using SmartHiring.Core.Specifications;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SmartHiring.Core.Repositories
{

    public interface IGenericRepository<T> where T : BaseEntity
    {
		#region Without Spec
		Task<IEnumerable<T>> GetAllAsync();
		Task<T> GetByIdAsync(int id);
		#endregion

		#region With Spec
		Task<IEnumerable<T>> GetAllWithSpecAsync(ISpecifications<T> Spec);
		Task<T> GetByEntityWithSpecAsync(ISpecifications<T> Spec);
		#endregion


		Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate);

		Task AddAsync(T entity);
		Task UpdateAsync(T entity);
		Task DeleteAsync(T entity);

		Task<IEnumerable<Application>> GetApplicationsByJobIdAsync(int jobId);
        Task<string> GetApplicationStatusAsync(int applicationId);
        Task SaveChangesAsync();

    }
}
