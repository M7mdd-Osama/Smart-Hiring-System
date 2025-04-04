
using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
    public class NotesByCompanySpec : BaseSpecifications<Note>
    {
        public NotesByCompanySpec(int companyId, int? postId = null)
            : base(n => (n.User.HRCompany.Id == companyId || n.User.ManagedCompany.Id == companyId) &&
                        (postId == null || n.PostId == postId))
        {
            AddInclude(n => n.User);
            AddInclude(n => n.Post);
            AddOrderBy(n => n.CreatedAt);
        }
    }
}
