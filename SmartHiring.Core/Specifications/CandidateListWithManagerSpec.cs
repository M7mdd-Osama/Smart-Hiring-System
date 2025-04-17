using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
    public class CandidateListWithManagerSpec : BaseSpec<CandidateList>
    {
        public CandidateListWithManagerSpec(int candidateListId)
            : base(cl => cl.Id == candidateListId)
        {
            AddInclude(cl => cl.Manager);
            AddIncludeString("Manager.ManagedCompany");
        }
    }
}
