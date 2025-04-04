using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
    public class NoteWithUserSpec : BaseSpecifications<Note>
    {
        public NoteWithUserSpec(int noteId)
            : base(n => n.Id == noteId)
        {
            AddInclude(n => n.User);
        }
    }
}
