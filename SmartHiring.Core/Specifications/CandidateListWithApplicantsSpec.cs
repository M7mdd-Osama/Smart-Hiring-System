﻿using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
    public class CandidateListWithApplicantsSpec : BaseSpec<CandidateList>
    {
        public CandidateListWithApplicantsSpec(int postId)
            : base(cl => cl.PostId == postId && cl.Status == "Accepted")
        {
            AddInclude(cl => cl.CandidateListApplicants);
            AddIncludeString("CandidateListApplicants.Applicant");
            AddIncludeString("CandidateListApplicants.Applicant.Applications");
            AddInclude(cl => cl.Post);
        }
    }
}
