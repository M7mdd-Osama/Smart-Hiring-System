using Microsoft.EntityFrameworkCore;
using SmartHiring.Core.Entities;
using SmartHiring.Core.Repositories;
using SmartHiring.Core.Specifications;
using SmartHiring.Repository.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHiring.Repository
{
	public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
	{

		private readonly SmartHiringContext _dbContext;

		public GenericRepository(SmartHiringContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<IEnumerable<T>> GetAllWithSpecAsync(ISpecifications<T> Spec)
		{
			return await ApplySpecification(Spec).ToListAsync();
		}

		public async Task<T> GetByIdWithSpecAsync(ISpecifications<T> Spec)
		{
			return await ApplySpecification(Spec).FirstOrDefaultAsync();
		}
		private IQueryable<T> ApplySpecification(ISpecifications<T> Spec)
		{
			return SpecificationEvaluator<T>.GetQuery(_dbContext.Set<T>(), Spec);
		}
	}
}