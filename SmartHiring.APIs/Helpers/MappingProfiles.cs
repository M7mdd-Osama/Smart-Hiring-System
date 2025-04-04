﻿using AutoMapper;
using Microsoft.AspNetCore.Identity;
using SmartHiring.APIs.DTOs;
using SmartHiring.APIs.Helpers;
using SmartHiring.Core.Entities;
using SmartHiring.Core.Entities.Identity;

public class MappingProfiles : Profile
{

	public MappingProfiles()
	{
		#region For Admin API

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
            .ForMember(dest => dest.CareerLevels, opt => opt.MapFrom(src => src.PostCareerLevels.Select(cl => cl.CareerLevel.LevelName)))
            .ForMember(dest => dest.HasUnreadNotes, opt => opt.MapFrom(src => src.Notes.Any(note => !note.IsSeen && note.UserId != src.HRId)));

        CreateMap<PostCreationDto, Post>()
			.ForMember(dest => dest.PostDate, opt => opt.MapFrom(src => DateTime.UtcNow))
			.ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => "Pending Payment"))
			.ForMember(dest => dest.HRId, opt => opt.Ignore())
			.ForMember(dest => dest.CompanyId, opt => opt.Ignore())
			.ForMember(dest => dest.PostJobCategories, opt => opt.MapFrom(src => src.JobCategories.Select(name => new PostJobCategory { JobCategory = new JobCategory { Name = name } })))
			.ForMember(dest => dest.PostJobTypes, opt => opt.MapFrom(src => src.JobTypes.Select(id => new PostJobType { JobTypeId = id })))
			.ForMember(dest => dest.PostWorkplaces, opt => opt.MapFrom(src => src.Workplaces.Select(id => new PostWorkplace { WorkplaceId = id })))
			.ForMember(dest => dest.PostCareerLevels, opt => opt.MapFrom(src => src.CareerLevels.Select(id => new PostCareerLevel { CareerLevelId = id })))
			.ForMember(dest => dest.PostSkills, opt => opt.MapFrom(src => src.Skills.Select(name => new PostSkill { Skill = new Skill { SkillName = name } })));

		CreateMap<Post, PostPaymentDto>()
			.ForMember(dest => dest.PaymentIntentId, opt => opt.NullSubstitute(null))
			.ForMember(dest => dest.ClientSecret, opt => opt.NullSubstitute(null));

		CreateMap<PostUpdateDto, Post>()
			.ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<Application, ApplicationDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.ApplicantName, opt => opt.MapFrom(src => $"{src.Applicant.FName} {src.Applicant.LName}"))
			.ForMember(dest => dest.AgencyName, opt => opt.MapFrom(src => src.Agency.AgencyName));

        CreateMap<Application, CandidateForManagerDto>()
				.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ApplicantId))
				.ForMember(dest => dest.ApplicantName, opt => opt.MapFrom(src => $"{src.Applicant.FName} {src.Applicant.LName}"))
				.ForMember(dest => dest.Rank, opt => opt.Ignore());

		CreateMap<CandidateList, CandidateListDto>()
			.ForMember(dest => dest.JobTitle, opt => opt.MapFrom(src => src.Post.JobTitle))
			.ForMember(dest => dest.TotalApplicants, opt => opt.MapFrom(src => src.CandidateListApplicants.Count))
			.ForMember(dest => dest.Remaining, opt => opt.MapFrom(src =>
				src.CandidateListApplicants.Count(a =>
					!src.Post.Interviews.Any(i =>
						i.ApplicantId == a.ApplicantId &&
						(i.InterviewStatus == InterviewStatus.Under_Interview ||
						 i.InterviewStatus == InterviewStatus.Hired ||
						 i.InterviewStatus == InterviewStatus.Rejected)
					)
				)
			))
			.ForMember(dest => dest.Progress, opt => opt.MapFrom(src =>
				src.CandidateListApplicants.Count == 0 ? 0 :
				(int)((src.CandidateListApplicants.Count(a =>
					src.Post.Interviews.Any(i =>
						i.ApplicantId == a.ApplicantId &&
						(i.InterviewStatus == InterviewStatus.Under_Interview ||
						 i.InterviewStatus == InterviewStatus.Hired ||
						 i.InterviewStatus == InterviewStatus.Rejected)
					)
				) / (double)src.CandidateListApplicants.Count) * 100)
			));

		CreateMap<CandidateListApplicant, CandidateListApplicantDto>()
			.ForMember(dest => dest.ApplicantId, opt => opt.MapFrom(src => src.ApplicantId))
			.ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.Applicant.FName} {src.Applicant.LName}"))
			.ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Applicant.Email))
			.ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Applicant.Phone))
			.ForMember(dest => dest.CV_Link, opt => opt.MapFrom(src =>
				src.Applicant.Applications.Any(a => a.PostId == src.CandidateList.PostId)
				? src.Applicant.Applications.First(a => a.PostId == src.CandidateList.PostId).CV_Link
				: ""
			))
			.ForMember(dest => dest.InterviewStatus, opt => opt.MapFrom(src => InterviewStatus.Pending.ToString()));

		CreateMap<InterviewSchedulingDto, Interview>()
			.ForMember(dest => dest.InterviewStatus, opt => opt.MapFrom(src => InterviewStatus.Under_Interview));

		CreateMap<UpdateInterviewStatusDto, Interview>()
			.ForMember(dest => dest.InterviewStatus,
				opt => opt.MapFrom(src => src.Status == "Hired" ? InterviewStatus.Hired : InterviewStatus.Rejected));

        CreateMap<Company, CompanyMembersDto>()
                .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.BusinessEmail, opt => opt.MapFrom(src => src.BusinessEmail))
            .ForMember(dest => dest.LogoUrl, O => O.MapFrom<PictureUrlResolver<Company, CompanyMembersDto>>())
                .ForMember(dest => dest.HR, opt => opt.MapFrom(src => src.HR))
                .ForMember(dest => dest.Manager, opt => opt.MapFrom(src => src.Manager));

        CreateMap<AppUser, MembersInfoDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.HRCompany != null ? "HR" : "Manager"));

        CreateMap<Note, NoteDto>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.ToString("yyyy-MM-dd HH:mm")))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom((src, dest, destMember, context) =>
            {
                var userEmail = context.Items["UserEmail"] as string;
                var roles = context.Items["UserRoles"] as IList<string>;

                if (userEmail == src.User.Email)
                    return "Me";

                // بناءً على الأدوار الموجودة:
                if (roles?.Contains("HR") == true) 
                    return "Manager";

                return "HR"; 
            }))
            .ForMember(dest => dest.IsSeen, opt => opt.MapFrom((src, dest, destMember, context) =>
            {
                var userEmail = context.Items["UserEmail"] as string;
                return src.User.Email == userEmail ? true : src.IsSeen;
            }));

        CreateMap<CreateNoteDto, Note>();

		#endregion

		#region For Agency API

		CreateMap<Post, PostToReturnForAgencyDto>()
			.ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company.Name))
			.ForMember(dest => dest.LogoUrl, opt => opt.MapFrom<PictureUrlResolver<Post, PostToReturnForAgencyDto>>())
			.ForMember(dest => dest.JobCategories, opt => opt.MapFrom(src => src.PostJobCategories.Select(c => c.JobCategory.Name)))
			.ForMember(dest => dest.JobTypes, opt => opt.MapFrom(src => src.PostJobTypes.Select(t => t.JobType.TypeName)))
			.ForMember(dest => dest.Workplaces, opt => opt.MapFrom(src => src.PostWorkplaces.Select(w => w.Workplace.WorkplaceType)))
			.ForMember(dest => dest.Skills, opt => opt.MapFrom(src => src.PostSkills.Select(s => s.Skill.SkillName)))
			.ForMember(dest => dest.CareerLevels, opt => opt.MapFrom(src => src.PostCareerLevels.Select(cl => cl.CareerLevel.LevelName)));

		CreateMap<EditAgencyDto, AppUser>()
			.ForMember(dest => dest.AgencyName, opt => opt.MapFrom(src => src.AgencyName))
			.ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
			.ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber));

        CreateMap<AddressDto, Address>()
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
            .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country));

        CreateMap<Interview, HiredApplicantDto>()
            .ForMember(d => d.FullName, o => o.MapFrom(s => s.Applicant.FName + " " + s.Applicant.LName))
            .ForMember(d => d.Email, o => o.MapFrom(s => s.Applicant.Email))
            .ForMember(d => d.Phone, o => o.MapFrom(s => s.Applicant.Phone))
            .ForMember(d => d.CompanyName, o => o.MapFrom(s => s.HR.HRCompany.Name));

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
				.ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.InterviewStatus.ToString()));

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

	}
}

