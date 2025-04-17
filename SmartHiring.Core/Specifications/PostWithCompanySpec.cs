using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
    public class PostWithCompanySpec : BaseSpec<Post>
    {
        public PostWithCompanySpec(PostSpecParams Params, int? companyId, string userRole, string userId, bool onlyPaid = false)
            : base(P =>
            (string.IsNullOrEmpty(Params.Search) || P.JobTitle.ToLower().Contains(Params.Search))
            &&
            (!companyId.HasValue || P.CompanyId == companyId)
            &&
            (!Params.typeId.HasValue || P.PostJobTypes.Any(jt => jt.JobTypeId == Params.typeId))
            &&
            (!Params.careerLevelId.HasValue || P.PostCareerLevels.Any(cl => cl.CareerLevelId == Params.careerLevelId))
            &&
            (!Params.workplaceId.HasValue || P.PostWorkplaces.Any(wp => wp.WorkplaceId == Params.workplaceId))
            &&
            (userRole == "HR" || P.PaymentStatus == "Paid" || P.SavedPosts.Any(sp => sp.UserId == userId))
            &&
            (userRole == "HR" || P.PaymentStatus == "Paid" || !onlyPaid || P.PaymentStatus == "Paid")
            )
        {
            AddIncludes();

            if (!string.IsNullOrEmpty(Params.Sort))
            {
                switch (Params.Sort.ToLower())
                {
                    case "postdateasc":
                        AddOrderBy(p => p.PostDate);
                        break;
                    case "postdatedesc":
                        AddOrderByDesc(p => p.PostDate);
                        break;
                    case "applicationsasc":
                        AddOrderBy(p => p.Applications.Count);
                        break;
                    case "applicationsdesc":
                        AddOrderByDesc(p => p.Applications.Count);
                        break;
                    default:
                        AddOrderByDesc(p => p.PostDate);
                        break;
                }
            }
            else
            {
                AddOrderByDesc(p => p.PostDate);
            }

            ApplyPagination(Params.PageSize * (Params.PageIndex - 1), Params.PageSize);
        }
        public PostWithCompanySpec(int postId)
            : base(p => p.Id == postId)
        {
            AddIncludes();
        }

        public PostWithCompanySpec(List<int> postIds)
            : base(p => postIds.Contains(p.Id))
        {
            AddIncludes();
        }

        private void AddIncludes()
        {
            AddInclude(p => p.Company);
            AddInclude(p => p.HR);
            AddInclude(p => p.PostJobCategories);
            AddIncludeString("PostJobCategories.JobCategory");
            AddInclude(p => p.PostJobTypes);
            AddIncludeString("PostJobTypes.JobType");
            AddInclude(p => p.PostWorkplaces);
            AddIncludeString("PostWorkplaces.Workplace");
            AddInclude(p => p.PostSkills);
            AddIncludeString("PostSkills.Skill");
            AddInclude(p => p.PostCareerLevels);
            AddIncludeString("PostCareerLevels.CareerLevel");
            AddInclude(p => p.Applications);
            AddInclude(p => p.CandidateLists);
            AddInclude(p => p.Notes);
        }
    }
}