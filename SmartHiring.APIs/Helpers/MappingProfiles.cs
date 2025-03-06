using AutoMapper;
using SmartHiring.APIs.DTOs;
using SmartHiring.Core.Entities;
using SmartHiring.Core.Entities.Identity;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        // ✅ Mapping Post → PostToReturnDto
        CreateMap<Post, PostToReturnDto>()
            .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.HR.HRCompany.Name))
            .ForMember(dest => dest.PostDate, opt => opt.Ignore()) // Manually controlled
            .ForMember(dest => dest.JobStatus, opt => opt.MapFrom(src => "Open")); // Default value

        // ✅ Mapping PostToReturnDto → Post
        CreateMap<PostToReturnDto, Post>()
            .ForMember(dest => dest.Id, opt => opt.Ignore()) // ID is auto-generated
            .ForMember(dest => dest.PostDate, opt => opt.Ignore()) // Manually controlled
            .ForMember(dest => dest.HR, opt => opt.Ignore()); // HR will be assigned later

        // ✅ Mapping Interview → CandidateReportToReturnDto
        CreateMap<Interview, CandidateReportToReturnDto>()
            .ForMember(d => d.Name, o => o.MapFrom(s => $"{s.Applicant.FName} {s.Applicant.LName}"))
            .ForMember(d => d.AverageScore, o => o.MapFrom(s => s.Score))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.InterviewStatus.ToString()));

        // ✅ Mapping Interview → InterviewDto
        CreateMap<Interview, InterviewDto>()
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date.Date))
            .ForMember(dest => dest.Time, opt => opt.MapFrom(src => src.Time.ToString(@"hh\:mm")));

        // ✅ Mapping InterviewDto → Interview
        CreateMap<InterviewDto, Interview>()
            .ForMember(dest => dest.InterviewStatus, opt => opt.Ignore())
            .ForMember(dest => dest.Score, opt => opt.Ignore())
            .ForMember(dest => dest.Time, opt => opt.MapFrom(src => TimeSpan.Parse(src.Time)));

        // ✅ Mapping Company → CompanyDto
        CreateMap<Company, CompanyDto>()
            .ForMember(dest => dest.HRCount, opt => opt.MapFrom(src => src.HR != null ? 1 : 0)) // Fix HR count logic
            .ForMember(dest => dest.PostCount, opt => opt.MapFrom(src => src.Posts.Count));

        // ✅ Mapping CompanyDto → Company
        CreateMap<CompanyDto, Company>()
            .ForMember(dest => dest.HR, opt => opt.Ignore()) // HR will be assigned later
            .ForMember(dest => dest.Posts, opt => opt.Ignore());

        // ✅ Mapping HRDto → AppUser
        CreateMap<HRDto, AppUser>()
           .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email)) // Ensure UserName is set correctly
           .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
           .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
           .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
           .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
           .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()); // Avoid setting password hash directly

        // ✅ Mapping AppUser → HRDto (optional, for returning user data)
        CreateMap<AppUser, HRDto>()
           .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
           .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
           .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
           .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber));
    }
}
