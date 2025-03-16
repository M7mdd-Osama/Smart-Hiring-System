using AutoMapper;
using SmartHiring.APIs.DTOs;
using SmartHiring.APIs.Helpers;
using SmartHiring.Core.Entities;
using SmartHiring.Core.Entities.Identity;

public class MappingProfiles : Profile
{

	public MappingProfiles()
	{
		#region For Admin Controller

		CreateMap<Company, CompanyDto>()
			.ForMember(dest => dest.HRName, opt => opt.MapFrom(src => src.HR != null ? src.HR.DisplayName : null))
			.ForMember(dest => dest.HREmail, opt => opt.MapFrom(src => src.HR != null ? src.HR.Email : null))
			.ForMember(dest => dest.ManagerName, opt => opt.MapFrom(src => src.Manager != null ? src.Manager.DisplayName : null))
			.ForMember(dest => dest.ManagerEmail, opt => opt.MapFrom(src => src.Manager != null ? src.Manager.Email : null))
			.ForMember(d => d.LogoUrl, O => O.MapFrom<PictureUrlResolver<Company, CompanyDto>>());

		CreateMap<AppUser, AgencyDto>()
			.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
			.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.DisplayName))
			.ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));

		#endregion

		#region For HR API

		CreateMap<Post, PostToReturnDto>()
			.ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company.Name))
			.ForMember(dest => dest.LogoUrl, O => O.MapFrom<PictureUrlResolver<Post, PostToReturnDto>>())
			.ForMember(dest => dest.HRName, opt => opt.MapFrom(src => src.HR.DisplayName))
			.ForMember(dest => dest.TotalApplications, opt => opt.MapFrom(src => src.Applications.Count))
			.ForMember(dest => dest.SelectedCandidates, opt => opt.MapFrom(src => src.CandidateLists.Count))
			.ForMember(dest => dest.JobCategories, opt => opt.MapFrom(src => src.PostJobCategories.Select(c => c.JobCategory.Name)))
			.ForMember(dest => dest.JobTypes, opt => opt.MapFrom(src => src.PostJobTypes.Select(t => t.JobType.TypeName)))
			.ForMember(dest => dest.Workplaces, opt => opt.MapFrom(src => src.PostWorkplaces.Select(w => w.Workplace.WorkplaceType)))
			.ForMember(dest => dest.Skills, opt => opt.MapFrom(src => src.PostSkills.Select(s => s.Skill.SkillName)))
			.ForMember(dest => dest.CareerLevels, opt => opt.MapFrom(src => src.PostCareerLevels.Select(cl => cl.CareerLevel.LevelName)));

		#endregion


		//CreateMap<Post, PostToReturnDto>()
		//	.ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.HR.HRCompany.Name))
		//	.ForMember(dest => dest.PostDate, opt => opt.Ignore())
		//	.ForMember(dest => dest.JobStatus, opt => opt.MapFrom(src => "Open"));

		//CreateMap<PostToReturnDto, Post>()
		//	.ForMember(dest => dest.Id, opt => opt.Ignore())
		//	.ForMember(dest => dest.PostDate, opt => opt.Ignore())
		//	.ForMember(dest => dest.HR, opt => opt.Ignore());

		CreateMap<Interview, CandidateReportToReturnDto>()
			.ForMember(d => d.Name, o => o.MapFrom(s => $"{s.Applicant.FName} {s.Applicant.LName}"))
			.ForMember(d => d.AverageScore, o => o.MapFrom(s => s.Score))
			.ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.InterviewStatus.ToString()));

		CreateMap<Interview, InterviewDto>()
			.ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date.Date))
			.ForMember(dest => dest.Time, opt => opt.MapFrom(src => src.Time.ToString(@"hh\:mm")));

		CreateMap<InterviewDto, Interview>()
			.ForMember(dest => dest.InterviewStatus, opt => opt.Ignore())
			.ForMember(dest => dest.Score, opt => opt.Ignore())
			.ForMember(dest => dest.Time, opt => opt.MapFrom(src => TimeSpan.Parse(src.Time)));

		CreateMap<Company, CompanyToReturnDto>()
			.ForMember(d => d.Manager, O => O.MapFrom(src => src.Manager.DisplayName))
			.ForMember(d => d.HR, O => O.MapFrom(src => src.HR.DisplayName))
			.ForMember(d => d.LogoUrl, O => O.MapFrom<PictureUrlResolver<Company, CompanyToReturnDto>>())
			.ForMember(d => d.Phone, O => O.MapFrom(src => src.CompanyPhones));


		CreateMap<CompanyCreateDto, Company>();
		CreateMap<CompanyUpdateDto, Company>();

		CreateMap<CompanyDto, Company>()
			.ForMember(dest => dest.HR, opt => opt.Ignore())
			.ForMember(dest => dest.Posts, opt => opt.Ignore())
			.ForMember(dest => dest.Manager, opt => opt.Ignore());

		CreateMap<HRDto, AppUser>()
			.ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
			.ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
			.ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
			.ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
			.ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
			.ForMember(dest => dest.PasswordHash, opt => opt.Ignore());

		CreateMap<AppUser, HRDto>()
			.ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
			.ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
			.ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
			.ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber));

		CreateMap<ManagerDto, AppUser>()
			.ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
			.ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
			.ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
			.ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
			.ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
			.ForMember(dest => dest.Address, opt => opt.Ignore())
			.ForMember(dest => dest.PasswordHash, opt => opt.Ignore());

		CreateMap<AppUser, ManagerDto>()
			.ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
			.ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
			.ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
			.ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
			.ForMember(dest => dest.Address, opt => opt.MapFrom(src =>
				src.Address != null ? $"{src.Address.City}, {src.Address.Country}" : "N/A"));

		CreateMap<Post, PostPaymentDto>()
			.ForMember(dest => dest.PaymentIntentId, opt => opt.NullSubstitute(null))
			.ForMember(dest => dest.ClientSecret, opt => opt.NullSubstitute(null));
	}
}

