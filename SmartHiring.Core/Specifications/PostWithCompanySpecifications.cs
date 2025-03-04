using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
    public class PostWithCompanySpecifications : BaseSpecifications<Post>
    {
        // ✅ جلب جميع الوظائف مع بيانات الشركة
        public PostWithCompanySpecifications()
        {
            AddInclude(p => p.HR.HRCompany);
        }

        // ✅ جلب وظيفة معينة حسب الـ Id مع بيانات الشركة
        public PostWithCompanySpecifications(int id)
            : base(p => p.Id == id)
        {
            AddInclude(p => p.HR.HRCompany);
        }
    }
}
