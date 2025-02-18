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
					.ForMember(d => d.CompanyName, O => O.MapFrom(S => S.HR.Company.Name));

			CreateMap<Interview, CandidateReportToReturnDto>()
				.ForMember(d => d.Name, o => o.MapFrom(s => s.Applicant.FName + " " + s.Applicant.LName))
				.ForMember(d => d.AverageScore, o => o.MapFrom(s => s.Score))
				.ForMember(d => d.Status, o => o.MapFrom(s => s.InterviewStatus));
		}
	}
}