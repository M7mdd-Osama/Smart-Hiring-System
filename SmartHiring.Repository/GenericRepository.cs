using Microsoft.EntityFrameworkCore;
using SmartHiring.Core.Entities;
using SmartHiring.Core.Repositories;
using SmartHiring.Core.Specifications;
using SmartHiring.Repository.Data;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SmartHiring.Repository
{

	public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
	{
		private readonly SmartHiringDbContext _dbContext;

		public GenericRepository(SmartHiringDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		#region Without Spec
		public async Task<IEnumerable<T>> GetAllAsync()
		=> await _dbContext.Set<T>().ToListAsync();

		public async Task<T> GetByIdAsync(int id)
		=> await _dbContext.Set<T>().FindAsync(id);

		#endregion

		#region With Spec
		public async Task<IEnumerable<T>> GetAllWithSpecAsync(ISpecifications<T> Spec)
		=> await ApplySpecification(Spec).ToListAsync();

		public async Task<T> GetByEntityWithSpecAsync(ISpecifications<T> Spec)
		=> await ApplySpecification(Spec).FirstOrDefaultAsync();

		private IQueryable<T> ApplySpecification(ISpecifications<T> Spec)
		=> SpecificationEvaluator<T>.GetQuery(_dbContext.Set<T>(), Spec);

		#endregion

		public async Task UpdateAsync(T entity)
		{
			_dbContext.Set<T>().Update(entity);
			await _dbContext.SaveChangesAsync();
		}


		public async Task<IEnumerable<Application>> GetApplicationsByJobIdAsync(int jobId)
		{
			return await _dbContext.Applications.Where(a => a.PostId == jobId).ToListAsync();
		}

		public async Task<string> GetApplicationStatusAsync(int applicationId)
		{
			var application = await _dbContext.Applications.FindAsync(applicationId);
			return application != null ? (application.IsShortlisted ? "Approved" : "Rejected") : null;
		}



		public async Task AddAsync(T entity)
		{
			try
			{
				await _dbContext.Set<T>().AddAsync(entity);
				await _dbContext.SaveChangesAsync();

				await _dbContext.Entry(entity).GetDatabaseValuesAsync();
			}
			catch (DbUpdateException dbEx)
			{
				throw new Exception($"خطأ أثناء حفظ البيانات: {dbEx.InnerException?.Message ?? dbEx.Message}");
			}
			catch (Exception ex)
			{
				throw new Exception($"خطأ غير متوقع: {ex.Message}");
			}
		}


		public async Task DeleteAsync(T entity)
		{
			if (entity == null)
				throw new ArgumentNullException(nameof(entity));

			_dbContext.Set<T>().Remove(entity);
			await _dbContext.SaveChangesAsync();
		}

		public async Task SaveChangesAsync()
		{
			await _dbContext.SaveChangesAsync();
		}

		public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate)
		{
			return await _dbContext.Set<T>().Where(predicate).ToListAsync();
		}
	}
}

