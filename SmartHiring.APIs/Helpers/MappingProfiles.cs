using AutoMapper;
using SmartHiring.APIs.DTOs;
using SmartHiring.Core.Entities;
using SmartHiring.Core.Entities.Identity;

public class MappingProfiles : Profile
{

    public MappingProfiles()
    {
        CreateMap<Post, PostToReturnDto>()
            .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.HR.HRCompany.Name))
            .ForMember(dest => dest.PostDate, opt => opt.Ignore())
            .ForMember(dest => dest.JobStatus, opt => opt.MapFrom(src => "Open"));

        CreateMap<PostToReturnDto, Post>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PostDate, opt => opt.Ignore())
            .ForMember(dest => dest.HR, opt => opt.Ignore());

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

        CreateMap<Company, CompanyDto>()
            .ForMember(dest => dest.HRCount, opt => opt.MapFrom(src => src.HR != null ? 1 : 0))
            .ForMember(dest => dest.PostCount, opt => opt.MapFrom(src => src.Posts.Count));
            
        CreateMap<Company, UserToReturnDto>()
            .ForMember(d => d.Admin, O => O.MapFrom(S => S.Admin.Name))
            .ForMember(d => d.Manager, O => O.MapFrom(S => S.Manager.Name));

            CreateMap<Company, CompanyToReturnDto>(); // تحقق من أن الحقول متطابقة
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
            .ForMember(dest => dest.Address, opt => opt.Ignore()) // ✅ تجاهل `Address`
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

