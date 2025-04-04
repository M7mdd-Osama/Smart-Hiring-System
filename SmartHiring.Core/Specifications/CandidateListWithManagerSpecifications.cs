using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
    public class CandidateListWithManagerSpecifications : BaseSpecifications<CandidateList>
    {
        public CandidateListWithManagerSpecifications(int candidateListId)
            : base(cl => cl.Id == candidateListId)
        {
            AddInclude(cl => cl.Manager);
            AddIncludeString("Manager.ManagedCompany");
        }
    }
}
