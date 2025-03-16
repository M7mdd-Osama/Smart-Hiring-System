using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
	public class PostWithCompanySpecifications : BaseSpecifications<Post>
	{
		public PostWithCompanySpecifications(PostSpecParams Params)
			: base(P =>
			(!Params.typeId.HasValue || P.PostJobTypes.Any(jt => jt.JobTypeId == Params.typeId))
			&&
			(!Params.careerLevelId.HasValue || P.PostCareerLevels.Any(cl => cl.CareerLevelId == Params.careerLevelId))
			&&
			(!Params.workplaceId.HasValue || P.PostWorkplaces.Any(wp => wp.WorkplaceId == Params.workplaceId)))
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

		public PostWithCompanySpecifications(int jobId)
			: base(p => p.Id == jobId)
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
		}
	}
}
