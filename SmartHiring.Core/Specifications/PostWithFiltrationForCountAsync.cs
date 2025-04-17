using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
    public class PostWithFiltrationForCountAsync : BaseSpec<Post>
    {
        public PostWithFiltrationForCountAsync(PostSpecParams Params, int? companyId, string userRole, bool onlyPaid)
            : base(P =>
            (string.IsNullOrEmpty(Params.Search) || P.JobTitle.ToLower().Contains(Params.Search))
            &&
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