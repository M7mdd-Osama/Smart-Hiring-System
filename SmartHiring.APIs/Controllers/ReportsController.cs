using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartHiring.APIs.DTOs;
using SmartHiring.APIs.Errors;
using SmartHiring.Core.Entities;
using SmartHiring.Core.Entities.Identity;
using SmartHiring.Core.Repositories;
using SmartHiring.Core.Specifications;
using System.Security.Claims;

namespace SmartHiring.APIs.Controllers
{
    public class ReportsController : APIBaseController
    {
        private readonly IGenericRepository<Interview> _interviewRepo;
        private readonly UserManager<AppUser> _agencyRepo;
        private readonly IMapper _mapper;
        private readonly IGenericRepository<Post> _jobRepo;
        private readonly IGenericRepository<Application> _applicationRepo;



        public ReportsController(
            IGenericRepository<Interview> interviewRepo,
            IGenericRepository<Post> jobRepo,
            IGenericRepository<Application> applicationRepo,
            UserManager<AppUser> agencyRepo,
            IMapper mapper)
        {
            _interviewRepo = interviewRepo;
            _jobRepo = jobRepo;
            _applicationRepo = applicationRepo;
            _agencyRepo = agencyRepo;
            _mapper = mapper;
        }


        #region Get Interview Stage Report

        [Authorize(Roles = "HR,Manager")]
        [HttpGet("interview-stage-report")]
        public async Task<ActionResult<InterviewReportToReturnDto>> GetInterviewStageReport(DateTime fromDate, DateTime toDate)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized(new ApiResponse(401, "User email not found in token"));

            var user = await _agencyRepo.Users
                .Include(u => u.HRCompany)
                .Include(u => u.ManagedCompany)
                .FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
                return Unauthorized(new ApiResponse(401, "User not found"));

            var userCompany = user.HRCompany ?? user.ManagedCompany;
            if (userCompany == null)
                return Unauthorized(new ApiResponse(401, "User is not associated with a company"));

            var spec = new InterviewWithCandidateSpecifications(fromDate, toDate, userCompany.Id);
            var filteredInterviews = await _interviewRepo.GetAllWithSpecAsync(spec);

            var accepted = filteredInterviews.Count(i => i.InterviewStatus == InterviewStatus.Hired);
            var rejected = filteredInterviews.Count(i => i.InterviewStatus == InterviewStatus.Rejected);

            var mappedCandidates = _mapper.Map<List<CandidateReportToReturnDto>>(
                filteredInterviews.Where(i =>
                    i.InterviewStatus == InterviewStatus.Hired ||
                    i.InterviewStatus == InterviewStatus.Rejected
                )
            );

            var report = new InterviewReportToReturnDto
            {
                TotalCandidates = accepted + rejected,
                AcceptedCandidates = accepted,
                RejectedCandidates = rejected,
                Candidates = mappedCandidates
            };

            return Ok(report);
        }

        #endregion

        //[Authorize(Roles = "Manager,HR")]
        [HttpGet("top-agencies")]
        [ProducesResponseType(typeof(TopAgencyDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<TopAgencyDto>> GetTopAgencies()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized(new ApiResponse(401, "User email not found"));

            var user = await _agencyRepo.Users
                .Include(u => u.ManagedCompany)
                .FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
                return Unauthorized(new ApiResponse(401, "User not found"));

            int? companyId = null;

            if (User.IsInRole("Manager"))
            {
                companyId = user.ManagedCompany?.Id;
                if (companyId == null)
                    return BadRequest(new ApiResponse(400, "Manager not associated with a company"));
            }

            // Get hired interviews with their HR (agency)
            var spec = new HiredInterviewsWithHRSpec(companyId);
            var interviews = await _interviewRepo.GetAllWithSpecAsync(spec);

            // Group interviews by agency HR
            var grouped = interviews
                .Where(i => i.HR != null)
                .GroupBy(i => i.HR)
                .Select(g => new TopAgencyItemDto
                {
                    Id = g.Key.Id.ToString(),
                    Name = $"{g.Key.FirstName} {g.Key.LastName}",
                    JobAppliedNumber = g.Count()
                })
                .OrderByDescending(a => a.JobAppliedNumber)
                .ToList();

            var agencyUsers = await _agencyRepo.GetUsersInRoleAsync("Agency");

            var reportDto = new TopAgencyDto
            {
                TotalAgencies = agencyUsers.Count,
                TopAgenciesCount = grouped.Count,
                TopAgencies = grouped,

            };

            return Ok(reportDto);
        }



        [Authorize(Roles = "HR,Manager")]
        [HttpGet("jobs/fill-status")]
        [ProducesResponseType(typeof(JobFillStatusReportDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<JobFillStatusReportDto>> GetJobsFillStatusReport()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized(new ApiResponse(401, "User email not found"));

            var user = await _agencyRepo.Users
                .Include(u => u.HRCompany)
                .Include(u => u.ManagedCompany)
                .FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
                return Unauthorized(new ApiResponse(401, "User not found"));

            var company = user.HRCompany ?? user.ManagedCompany;

            if (company == null)
                return Unauthorized(new ApiResponse(401, "User is not associated with a company"));

            var spec = new JobsWithInterviewsSpec(company.Id);
            var jobs = await _jobRepo.GetAllWithSpecAsync(spec);

            var filledJobs = jobs
                .Where(j => j.Interviews.Any(i => i.InterviewStatus == InterviewStatus.Hired))
                .Select(j => j.JobTitle) // افترضنا إن فيه title للوظيفة
                .ToList();

            var unfilledJobs = jobs
                .Where(j => !j.Interviews.Any(i => i.InterviewStatus == InterviewStatus.Hired))
                .Select(j => j.JobTitle)
                .ToList();

            var report = new JobFillStatusReportDto
            {
                TotalPosts = jobs.Count(),
                FilledPosts = filledJobs.Count,
                UnfilledPosts = unfilledJobs.Count,
                FilledPostsData = filledJobs,
                UnfilledPostsData = unfilledJobs
            };

            return Ok(report);
        }



        //[Authorize(Roles = "Admin,HR")]
        [HttpGet("ai/acceptance-rejection-ratio/{jobId}")]
        [ProducesResponseType(typeof(AIScreeningReportDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<AIScreeningReportDto>> GetAIScreeningRatio(int jobId)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized(new ApiResponse(401, "User email not found"));

            var user = await _agencyRepo.Users
                .Include(u => u.HRCompany)
                .FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
                return Unauthorized(new ApiResponse(401, "User not found"));

            if (User.IsInRole("HR"))
            {
                // تأكد إن الـ Job بتاعة الشركة اللي الـ HR شغال فيها
                var jobSpec = new JobWithCompanySpec(jobId);
                var job = await _jobRepo.GetByEntityWithSpecAsync(jobSpec);

                if (job == null || job.CompanyId != user.HRCompany?.Id)
                    return Forbid(); // مش من حقه يشوف الوظيفة دي
            }

            var spec = new ApplicationsForJobSpec(jobId);
            var applications = await _applicationRepo.GetAllWithSpecAsync(spec);

            var accepted = applications.Count(a => a.IsShortlisted);
            var rejected = applications.Count(a => !a.IsShortlisted);

            var result = new AIScreeningReportDto
            {
                AcceptedByAI = accepted,
                RejectedByAI = rejected
            };

            return Ok(result);
        }

        //[Authorize(Roles = "HR")]
        [HttpGet("interview/success-rate/{jobId}")]
        [ProducesResponseType(typeof(InterviewSuccessRateDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<InterviewSuccessRateDto>> GetInterviewSuccessRate(int jobId)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized(new ApiResponse(401, "User email not found"));

            var user = await _agencyRepo.Users
                .Include(u => u.HRCompany)
                .FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
                return Unauthorized(new ApiResponse(401, "User not found"));

            // ربط مؤقت بشركة Nexus Soft لو مش مربوط بأي شركة
            if (user.HRCompany == null)
            {
                user.HRCompany = new Company { Id = 20, Name = "Nexus Soft" };
            }

            var jobSpec = new JobWithCompanySpec(jobId);
            var job = await _jobRepo.GetByEntityWithSpecAsync(jobSpec);

            if (job == null || job.CompanyId != user.HRCompany.Id)
                return Forbid(); // 403

            var spec = new InterviewsForJobSpec(jobId);
            var interviews = await _interviewRepo.GetAllWithSpecAsync(spec);

            var total = interviews.Count();
            var hired = interviews.Count(i => i.InterviewStatus == InterviewStatus.Hired);
            var failed = total - hired;
            var rate = total == 0 ? 0 : Math.Round((double)hired / total * 100, 2);

            // استخراج أسماء المتقدمين حسب الحالة
            var successfulApplicants = interviews
                .Where(i => i.InterviewStatus == InterviewStatus.Hired && i.Applicant != null)
                .Select(i => $"{i.Applicant.FName} {i.Applicant.LName}")
                .ToList();

            var failedApplicants = interviews
                .Where(i => i.InterviewStatus != InterviewStatus.Hired && i.Applicant != null)
                .Select(i => $"{i.Applicant.FName} {i.Applicant.LName}")
                .ToList();

            var result = new InterviewSuccessRateDto
            {
                TotalInterviews = total,
                HiredCount = hired,
                SuccessRatePercentage = rate,
                SuccessfulApplicants = successfulApplicants,
                FailedApplicants = failedApplicants
            };

            return Ok(result);
        }


        [Authorize(Roles = "Manager,HR")]
        [HttpGet("applications/{companyId}")]
        [ProducesResponseType(typeof(PostApplicationsDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<PostApplicationsDto>> GetPostApplications(int companyId)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized(new ApiResponse(401, "User email not found"));

            var user = await _agencyRepo.Users
                .Include(u => u.ManagedCompany)
                .Include(u => u.HRCompany)
                .FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
                return Unauthorized(new ApiResponse(401, "User not found"));

            var userCompanyId = user.ManagedCompany?.Id ?? user.HRCompany?.Id;

            if (userCompanyId != companyId)
                return Forbid();

            var spec = new PostsWithApplicationsSpec(companyId);
            var posts = await _jobRepo.GetAllWithSpecAsync(spec);

            var jobData = posts.Select(p => new JobApplicationDataDto
            {
                JobName = p.JobTitle,
                ApplicationNumber = p.Applications?.Count ?? 0,
                JobId = p.Id.ToString()
            }).ToList();

            var totalApplications = jobData.Sum(j => j.ApplicationNumber);

            var report = new PostApplicationsDto
            {
                TotalApplications = totalApplications,
                TotalJobs = jobData.Count,
                Jobs = jobData
            };

            return Ok(report);
        }




        //[Authorize(Roles = "HR")]
        [HttpGet("jobs/top-applications-by-rank")]
        [ProducesResponseType(typeof(RankedApplicationsReportDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<RankedApplicationsReportDto>> GetTopApplicationsByRank()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized(new ApiResponse(401, "User email not found"));

            var user = await _agencyRepo.Users
                .Include(u => u.HRCompany)
                .FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
                return Unauthorized(new ApiResponse(401, "User not found"));

            var company = user.HRCompany;
            if (company == null)
                return Unauthorized(new ApiResponse(401, "User is not associated with a company"));

            // جلب الـ Applications من وظائف الشركة
            var spec = new ApplicationsWithJobSpec(company.Id);
            var applications = await _applicationRepo.GetAllWithSpecAsync(spec);

            var topApplications = applications
.Where(a => a.RankScore > 0) // أو أي شرط منطقي يناسبك
                .OrderByDescending(a => a.RankScore)
                .ToList();

            var mapped = _mapper.Map<List<ApplicationRankedDto>>(topApplications);

            var report = new RankedApplicationsReportDto
            {
                TotalApplications = mapped.Count,
                TopApplications = mapped
            };

            return Ok(report);
        }


        [Authorize(Roles = "Manager,HR")]
        [HttpGet("jobs/most-vs-least-applied")]
        [ProducesResponseType(typeof(JobApplicationComparisonDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<JobApplicationComparisonDto>> GetMostVsLeastAppliedJobs()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized(new ApiResponse(401, "User email not found"));

            var user = await _agencyRepo.Users
                .Include(u => u.ManagedCompany)
                .Include(u => u.HRCompany)
                .FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
                return Unauthorized(new ApiResponse(401, "User not found"));

            var companyId = user.ManagedCompany?.Id ?? user.HRCompany?.Id;
            if (companyId == null)
                return Forbid();

            var spec = new PostsWithApplicationsSpec(companyId.Value);
            var posts = await _jobRepo.GetAllWithSpecAsync(spec);

            if (!posts.Any())
            {
                return Ok(new JobApplicationComparisonDto
                {
                    TotalJobsApplied = 0,
                    MostJobCount = 0,
                    LeastJobCount = 0,
                    CandidatesData = new CandidatesDataDto
                    {
                        MostJobCandidates = new List<string>(),
                        LeastJobCandidates = new List<string>()
                    }
                });
            }

            var ordered = posts
                .Select(p => new
                {
                    Count = p.Applications?.Count ?? 0,
                    Candidates = p.Applications?
                        .Select(a => $"{a.Applicant.FName} {a.Applicant.LName}")
                        .ToList() ?? new List<string>()
                })
                .OrderByDescending(p => p.Count)
                .ToList();

            var most = ordered.First();
            var least = ordered.Last();

            var result = new JobApplicationComparisonDto
            {
                TotalJobsApplied = ordered.Sum(p => p.Count),
                MostJobCount = most.Count,
                LeastJobCount = least.Count,
                CandidatesData = new CandidatesDataDto
                {
                    MostJobCandidates = most.Candidates,
                    LeastJobCandidates = least.Candidates
                }
            };

            return Ok(result);
        }



        //[Authorize(Roles = "HR,Manager")]
        [HttpGet("ai-screening/summary/{jobId}")]
        [ProducesResponseType(typeof(AIScreeningSummaryDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<AIScreeningSummaryDto>> GetAIScreeningSummary(int jobId)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized(new ApiResponse(401, "User email not found"));

            var user = await _agencyRepo.Users
                .Include(u => u.HRCompany)
                .Include(u => u.ManagedCompany)
                .FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
                return Unauthorized(new ApiResponse(401, "User not found"));

            // تحقق من ملكية الوظيفة
            var jobSpec = new JobWithCompanySpec(jobId);
            var job = await _jobRepo.GetByEntityWithSpecAsync(jobSpec);

            var userCompanyId = user.HRCompany?.Id ?? user.ManagedCompany?.Id;

            if (job == null || job.CompanyId != userCompanyId)
                return Forbid();

            var spec = new ApplicationsWithApplicantSpec(jobId);
            var applications = await _applicationRepo.GetAllWithSpecAsync(spec);

            var accepted = applications
                .Where(a => a.IsShortlisted)
                .Select(a => _mapper.Map<ApplicantDto>(a.Applicant))
                .ToList();

            var rejected = applications
                .Where(a => !a.IsShortlisted)
                .Select(a => _mapper.Map<ApplicantDto>(a.Applicant))
                .ToList();

            var result = new AIScreeningSummaryDto
            {
                AcceptedApplicants = accepted,
                RejectedApplicants = rejected
            };

            return Ok(result);
        }


    }
}