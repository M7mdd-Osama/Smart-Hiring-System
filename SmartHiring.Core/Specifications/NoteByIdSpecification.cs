using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
    public class NoteByIdSpecification : BaseSpec<Note>
    {
        public NoteByIdSpecification(int noteId, string userId)
            : base(n => n.Id == noteId && n.UserId == userId)
        {
            AddInclude(n => n.User);
        }
    }
}
