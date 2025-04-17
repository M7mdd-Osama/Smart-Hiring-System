using System.Collections;
using SmartHiring.Core;
using SmartHiring.Core.Repositories;
using SmartHiring.Repository.Data;

namespace SmartHiring.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SmartHiringDbContext _dbContext;
        private Hashtable _repositories;

        public UnitOfWork(SmartHiringDbContext dbContext)
        {
            _repositories = new Hashtable();
            _dbContext = dbContext;
        }
        public async Task<int> CompleteAsync()
        => await _dbContext.SaveChangesAsync();

        public ValueTask DisposeAsync()
        => _dbContext.DisposeAsync();

        public IGenericRepo<TEntity> Repository<TEntity>() where TEntity : class
        {
            var type = typeof(TEntity).Name;

            if(!_repositories.ContainsKey(type))
            {
                var Repository = new GenericRepository<TEntity>(_dbContext);
                _repositories.Add(type, Repository);
            }
            return _repositories[type] as IGenericRepo<TEntity>;
        }
    }
}
