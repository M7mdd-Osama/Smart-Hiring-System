using AutoMapper;
using SmartHiring.APIs.DTOs;
using SmartHiring.Core.Entities;

namespace SmartHiring.APIs.Helpers
{
	public class MappingProfiles : Profile
	{
		public MappingProfiles()
		{
			CreateMap<Post, PostToReturnDto>()
					.ForMember(d => d.CompanyName, O => O.MapFrom(S => S.HR.HRCompany.Name))
			.ForMember(dest => dest.PostDate, opt => opt.Ignore()) // سنتحكم فيه يدويًا
            .ForMember(dest => dest.JobStatus, opt => opt.MapFrom(src => "Open")); // تعيين قيمة افتراضية


            CreateMap<Interview, CandidateReportToReturnDto>()
				.ForMember(d => d.Name, o => o.MapFrom(s => s.Applicant.FName + " " + s.Applicant.LName))
				.ForMember(d => d.AverageScore, o => o.MapFrom(s => s.Score))
				.ForMember(d => d.Status, o => o.MapFrom(s => s.InterviewStatus))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.InterviewStatus.ToString()));

            CreateMap<Interview, InterviewDto>()
    .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date.Date))
    .ForMember(dest => dest.Time, opt => opt.MapFrom(src => src.Time.ToString(@"hh\:mm"))); // تحويل TimeSpan إلى hh:mm

            CreateMap<InterviewDto, Interview>()
                .ForMember(dest => dest.InterviewStatus, opt => opt.Ignore())
                .ForMember(dest => dest.Score, opt => opt.Ignore())
                .ForMember(dest => dest.Time, opt => opt.MapFrom(src => TimeSpan.Parse(src.Time))); // تحويل string إلى TimeSpan عند الإدخال

        }
    }
}