using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
    public class NoteByIdAndCompanySpec : BaseSpecifications<Note>
    {
        public NoteByIdAndCompanySpec(int noteId, int companyId)
            : base(n =>
                n.Id == noteId &&
                (n.User.HRCompany.Id == companyId || n.User.ManagedCompany.Id == companyId))
        {
            AddInclude(n => n.User);
        }
    }
}
