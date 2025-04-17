using Microsoft.EntityFrameworkCore;
using SmartHiring.Core.Repositories;
using SmartHiring.Core.Specifications;
using SmartHiring.Repository.Data;
using System.Linq.Expressions;

namespace SmartHiring.Repository
{
	public class GenericRepository<T> : IGenericRepo<T> where T : class
	{
		private readonly SmartHiringDbContext _dbContext;

		public GenericRepository(SmartHiringDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		#region Without Spec

		public async Task<IEnumerable<T>> GetAllAsync()
			=> await _dbContext.Set<T>().ToListAsync();

		public async Task<T> GetByIdAsync(object id)
			=> await _dbContext.Set<T>().FindAsync(id);

		#endregion

		#region With Spec

		public async Task<IEnumerable<T>> GetAllWithSpecAsync(ISpec<T> Spec)
			=> await ApplySpecification(Spec).ToListAsync();

		public async Task<T> GetByEntityWithSpecAsync(ISpec<T> Spec)
			=> await ApplySpecification(Spec).FirstOrDefaultAsync();

		private IQueryable<T> ApplySpecification(ISpec<T> Spec)
			=> SpecificationEvaluator<T>.GetQuery(_dbContext.Set<T>(), Spec);

		#endregion

		public async Task<int> GetCountWithSpecAsync(ISpec<T> Spec)
		=> await ApplySpecification(Spec).CountAsync();
		
		public async Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
		=> await _dbContext.Set<T>().FirstOrDefaultAsync(predicate);

		public async Task AddRangeAsync(IEnumerable<T> entities)
		=> await _dbContext.Set<T>().AddRangeAsync(entities);

		// ------------------------------------------------------

		public async Task UpdateAsync(T entity)
		{
			_dbContext.Set<T>().Update(entity);
			await _dbContext.SaveChangesAsync();
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
                throw new Exception($"Error while saving data: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Unexpected error: {ex.Message}");
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
        => await _dbContext.SaveChangesAsync();
        
        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate)
		=> await _dbContext.Set<T>().Where(predicate).ToListAsync();
		
	}
}

