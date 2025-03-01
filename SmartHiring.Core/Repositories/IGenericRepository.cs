using SmartHiring.Core.Entities;
using SmartHiring.Core.Specifications;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartHiring.Core.Repositories
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<T> GetByIdAsync(int id); // ✅ إضافة دالة جلب كيان حسب الـ ID
        Task<IEnumerable<T>> GetAllWithSpecAsync(ISpecifications<T> Spec);
        Task<T> GetByIdWithSpecAsync(ISpecifications<T> Spec);
        Task UpdateAsync(T entity); // ✅ إضافة دالة تحديث كيان معين

        // دوال خاصة بـ Application فقط
        Task<IEnumerable<Application>> GetApplicationsByJobIdAsync(int jobId);
        Task<string> GetApplicationStatusAsync(int applicationId);
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(T entity); // ✅ تأكد من وجود هذه الميثود
        Task DeleteAsync(T entity);
        Task SaveChangesAsync();
    }
}
