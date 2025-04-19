using AutoMapper;
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
            .ForMember(dest => dest.PhoneNumberHR, opt => opt.MapFrom(src => src.HR != null ? src.HR.PhoneNumber : null))
            .ForMember(dest => dest.ManagerName, opt => opt.MapFrom(src => src.Manager != null ? src.Manager.DisplayName : null))
            .ForMember(dest => dest.ManagerEmail, opt => opt.MapFrom(src => src.Manager != null ? src.Manager.Email : null))
            .ForMember(dest => dest.PhoneNumberManager, opt => opt.MapFrom(src => src.Manager != null ? src.Manager.PhoneNumber : null))
            .ForMember(d => d.LogoUrl, O => O.MapFrom<PictureUrlResolver<Company, CompanyDto>>());

        CreateMap<AppUser, AgencyDto>();

        CreateMap<CreateCompanyByAdminDto, Company>()
            .ForMember(dest => dest.LogoUrl, opt => opt.Ignore())
            .ForMember(dest => dest.EmailConfirmed, opt => opt.MapFrom(src => true));

        CreateMap<UpdateCompanyByAdminDto, Company>()
            .ForMember(dest => dest.LogoUrl, opt => opt.Ignore());

        CreateMap<CreateAgencyByAdminDto, AppUser>()
            .ForMember(dest => dest.EmailConfirmed, opt => opt.MapFrom(src => true));

        CreateMap<UpdateAgencyByAdminDto, AppUser>();

        #endregion

        #region For HR API

        CreateMap<Post, PostToReturnDto>()
            .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company.Name))
            .ForMember(dest => dest.LogoUrl, O => O.MapFrom<PictureUrlResolver<Post, PostToReturnDto>>())
            .ForMember(dest => dest.HRName, opt => opt.MapFrom(src => src.HR.DisplayName))
            .ForMember(dest => dest.TotalApplications, opt => opt.MapFrom(src => src.Applications.Count))
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
            .ForMember(dest => dest.ApplicantName, opt => opt.MapFrom(src => src.Applicant.GetFullName()))
            .ForMember(dest => dest.AgencyName, opt => opt.MapFrom(src => src.Agency.AgencyName));

        CreateMap<Application, CandidateForManagerDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ApplicantId))
                .ForMember(dest => dest.ApplicantName, opt => opt.MapFrom(src => src.Applicant.GetFullName()))
                .ForMember(dest => dest.Rank, opt => opt.Ignore());

        CreateMap<CandidateList, CandidateListWithApplicantsDto>()
            .ForMember(dest => dest.JobTitle, opt => opt.MapFrom(src => src.Post.JobTitle))
            .ForMember(dest => dest.TotalApplicants, opt => opt.MapFrom(src => src.CandidateListApplicants.Count))
            .ForMember(dest => dest.Remaining, opt => opt.MapFrom(src =>
                src.CandidateListApplicants.Count(a =>
                    !src.Post.Interviews.Any(i =>
                        i.ApplicantId == a.ApplicantId &&
                        (i.InterviewStatus == InterviewStatus.Hired ||
                         i.InterviewStatus == InterviewStatus.Rejected)
                    )
                )
            ))
            .ForMember(dest => dest.Progress, opt => opt.MapFrom(src =>
                src.CandidateListApplicants.Count == 0 ? 0 :
                (int)((src.CandidateListApplicants.Count(a =>
                    src.Post.Interviews.Any(i =>
                        i.ApplicantId == a.ApplicantId &&
                        (i.InterviewStatus == InterviewStatus.Hired ||
                         i.InterviewStatus == InterviewStatus.Rejected)
                    )
                ) / (double)src.CandidateListApplicants.Count) * 100)
            ));

        CreateMap<CandidateListApplicant, CandidateListApplicantDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Applicant.GetFullName()))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Applicant.Email))
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Applicant.Phone))
            .ForMember(dest => dest.CV_Link, opt => opt.MapFrom(src =>
                src.Applicant.Applications.Any(a => a.PostId == src.CandidateList.PostId)
                ? src.Applicant.Applications.First(a => a.PostId == src.CandidateList.PostId).CV_Link
                : ""))
            .ForMember(dest => dest.InterviewStatus, opt => opt.MapFrom(src =>
                src.CandidateList.Post.Interviews.Any(i => i.ApplicantId == src.ApplicantId)
                ? src.CandidateList.Post.Interviews.First(i => i.ApplicantId == src.ApplicantId).InterviewStatus.ToString()
                : InterviewStatus.Pending.ToString()))
            .ForMember(dest => dest.InterviewDate, opt => opt.MapFrom(src =>
                src.CandidateList.Post.Interviews.Any(i => i.ApplicantId == src.ApplicantId)
                ? src.CandidateList.Post.Interviews.First(i => i.ApplicantId == src.ApplicantId).Date
                : (DateTime?)null))
            .ForMember(dest => dest.InterviewTime, opt => opt.MapFrom(src =>
                src.CandidateList.Post.Interviews.Any(i => i.ApplicantId == src.ApplicantId)
                ? src.CandidateList.Post.Interviews.First(i => i.ApplicantId == src.ApplicantId).Time
                : (TimeSpan?)null))
            .ForMember(dest => dest.Location, opt => opt.MapFrom(src =>
                src.CandidateList.Post.Interviews.Any(i => i.ApplicantId == src.ApplicantId)
                ? src.CandidateList.Post.Interviews.First(i => i.ApplicantId == src.ApplicantId).Location
                : string.Empty))
            .ForMember(dest => dest.InterviewId, opt => opt.MapFrom(src =>
                src.CandidateList.Post.Interviews.Any(i => i.ApplicantId == src.ApplicantId)
                ? src.CandidateList.Post.Interviews.First(i => i.ApplicantId == src.ApplicantId).Id
                : (int?)null));

        CreateMap<InterviewSchedulingDto, Interview>()
            .ForMember(dest => dest.InterviewStatus, opt => opt.MapFrom(src => InterviewStatus.Under_Interview));

        CreateMap<UpdateInterviewStatusDto, Interview>()
            .ForMember(dest => dest.InterviewStatus,
                opt => opt.MapFrom(src => src.Status == "Hired" ? InterviewStatus.Hired : InterviewStatus.Rejected));

        CreateMap<Company, CompanyMembersDto>()
                .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.LogoUrl, O => O.MapFrom<PictureUrlResolver<Company, CompanyMembersDto>>());

        CreateMap<AppUser, MembersInfoDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.GetFullName()))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.HRCompany != null ? "HR" : "Manager"));

        CreateMap<Note, NoteDto>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.ToString("yyyy-MM-dd HH:mm")))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom((src, dest, destMember, context) =>
            {
                var userEmail = context.Items["UserEmail"] as string;
                var roles = context.Items["UserRoles"] as IList<string>;

                if (userEmail == src.User.Email)
                    return "Me";

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

        #region For Manager API

        CreateMap<Application, PendingCandidateListApplicantDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Applicant.GetFullName()));

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

        CreateMap<EditUserDto, AppUser>();

        CreateMap<AddressDto, Address>();

        CreateMap<Interview, HiredApplicantDto>()
            .ForMember(d => d.FullName, o => o.MapFrom(s => s.Applicant.GetFullName()))
            .ForMember(d => d.Email, o => o.MapFrom(s => s.Applicant.Email))
            .ForMember(d => d.ApplicantPhone, o => o.MapFrom(s => s.Applicant.Phone))
            .ForMember(d => d.CompanyPhone, o => o.MapFrom(s => s.HR.HRCompany.Phone))
            .ForMember(d => d.CompanyName, o => o.MapFrom(s => s.HR.HRCompany.Name))
            .ForMember(d => d.BusinessEmail, o => o.MapFrom(s => s.HR.HRCompany.BusinessEmail));

        CreateMap<SubmitApplicationDto, Applicant>();

        #endregion

        CreateMap<Interview, CandidateReportToReturnDto>()

            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Applicant.GetFullName()))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.InterviewStatus.ToString()));

        CreateMap<AppUser, TopAgencyItemDto>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.DisplayName));

        CreateMap<Application, ApplicationRankedDto>()

            .ForMember(dest => dest.CV_Link, opt => opt.MapFrom(src => src.CV_Link))
            .ForMember(dest => dest.Rank, opt => opt.MapFrom(src => src.RankScore));


        CreateMap<Applicant, ApplicantDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.GetFullName()))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));



        CreateMap<Company, CompanyCountReportDto>()
            .ForMember(dest => dest.TotalCompanies, opt => opt.Ignore()); 

        CreateMap<AppUser, AgencyCountReportDto>()
            .ForMember(dest => dest.TotalAgencies, opt => opt.Ignore());

        CreateMap<Post, JobClosedCountReportDto>()
            .ForMember(dest => dest.TotalClosedJobs, opt => opt.MapFrom(src => 1)); 

        CreateMap<Post, JobApplicationsCountDto>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.JobTitle))
            .ForMember(dest => dest.JobAppliedNumber, opt => opt.MapFrom(src => src.Applications.Count));


        CreateMap<Interview, PendingInterviewCandidateDto>()
            .ForMember(dest => dest.CandidateName, opt => opt.MapFrom(src => src.Applicant.GetFullName()))
            .ForMember(dest => dest.CandidateEmail, opt => opt.MapFrom(src => src.Applicant.Email))
            .ForMember(dest => dest.InterviewDate, opt => opt.MapFrom(src => src.Date))
            .ForMember(dest => dest.JobTitle, opt => opt.MapFrom(src => src.Post.JobTitle));


        CreateMap<Interview, CandidateReportToReturnDto>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Applicant.GetFullName()))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.InterviewStatus.ToString()));

        CreateMap<Post, JobApplicationsCountDto>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.JobTitle))
            .ForMember(dest => dest.JobAppliedNumber, opt => opt.MapFrom(src => src.Applications.Count));

        CreateMap<Company, CompanyPostCountDto>()
    .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Name))
    .ForMember(dest => dest.TotalPosts, opt => opt.MapFrom(src => src.Posts.Count));

    }
}