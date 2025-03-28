using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
	public class PostWithFiltrationForCountAsync : BaseSpecifications<Post>
	{
		public PostWithFiltrationForCountAsync(PostSpecParams Params, int? companyId, string userRole, bool onlyPaid)
			: base(P =>
			(!Params.typeId.HasValue || P.PostJobTypes.Any(jt => jt.JobTypeId == Params.typeId))
			&&
			(!Params.careerLevelId.HasValue || P.PostCareerLevels.Any(cl => cl.CareerLevelId == Params.careerLevelId))
			&&
			(!Params.workplaceId.HasValue || P.PostWorkplaces.Any(wp => wp.WorkplaceId == Params.workplaceId))
			&&
			(companyId == null || P.CompanyId == companyId)
			&&
			(!onlyPaid || P.PaymentStatus == "Paid")
			)
		{ }
	}
}