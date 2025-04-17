using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartHiring.APIs.DTOs;
using SmartHiring.APIs.Errors;
using SmartHiring.Core.Entities.Identity;
using SmartHiring.Core.Entities;
using SmartHiring.Core.Repositories;
using SmartHiring.Core.Specifications;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace SmartHiring.APIs.Controllers
{
    public class ReportsController : APIBaseController
    {
        private readonly IGenericRepo<Interview> _interviewRepo;
        private readonly IGenericRepo<Company> _companyRepo;
        private readonly UserManager<AppUser> _agencyRepo;
        private readonly UserManager<AppUser> _userManager;
        private readonly IGenericRepo<Post> _postRepo;
        private readonly IGenericRepo<Application> _applicationRepo;
        private readonly IMapper _mapper;

        public ReportsController(
            IGenericRepo<Interview> interviewRepo,
            IGenericRepo<Company> companyRepo,
            UserManager<AppUser> userManager,
            IGenericRepo<Post> postRepo,
            UserManager<AppUser> agencyRepo,
            IGenericRepo<Application> applicationRepo,
            IMapper mapper)
        {
            _interviewRepo = interviewRepo;
            _companyRepo = companyRepo;
            _userManager = userManager;
            _postRepo = postRepo;
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

            var user = await _userManager.Users
                .Include(u => u.HRCompany)
                .Include(u => u.ManagedCompany)
                .FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
                return Unauthorized(new ApiResponse(401, "User not found"));

            var userCompany = user.HRCompany ?? user.ManagedCompany;
            if (userCompany == null)
                return Unauthorized(new ApiResponse(401, "User is not associated with a company"));

            var spec = new InterviewWithCandidateSpec(fromDate, toDate, userCompany.Id);
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

        #region Mohsen

        #region Get Top Agencies Report
        [Authorize(Roles = "Manager,HR")]
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

            var spec = new HiredInterviewsWithHRSpec(companyId);
            var interviews = await _interviewRepo.GetAllWithSpecAsync(spec);

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
        #endregion

        #region Get Jobs Fill Status Report
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
            var jobs = await _postRepo.GetAllWithSpecAsync(spec);

            var filledJobs = jobs
                .Where(j => j.Interviews.Any(i => i.InterviewStatus == InterviewStatus.Hired))
                .Select(j => j.JobTitle)
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
        #endregion

        #region Get AI Screening Ratio
        [Authorize(Roles = "Admin,HR")]
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
                var jobSpec = new JobWithCompanySpec(jobId);
                var job = await _postRepo.GetByEntityWithSpecAsync(jobSpec);

                if (job == null || job.CompanyId != user.HRCompany?.Id)
                    return Forbid();
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
        #endregion

        #region Get Interview Success Rate
        [Authorize(Roles = "HR")]
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

            if (user.HRCompany == null)
            {
                user.HRCompany = new Company { Id = 20, Name = "Nexus Soft" };
            }

            var jobSpec = new JobWithCompanySpec(jobId);
            var job = await _postRepo.GetByEntityWithSpecAsync(jobSpec);

            if (job == null || job.CompanyId != user.HRCompany.Id)
                return Forbid();

            var spec = new InterviewsForJobSpec(jobId);
            var interviews = await _interviewRepo.GetAllWithSpecAsync(spec);

            var total = interviews.Count();
            var hired = interviews.Count(i => i.InterviewStatus == InterviewStatus.Hired);
            var failed = total - hired;
            var rate = total == 0 ? 0 : Math.Round((double)hired / total * 100, 2);

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
        #endregion

        #region Get Post Applications
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
            var posts = await _postRepo.GetAllWithSpecAsync(spec);

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
        #endregion

        #region Get Top Applications By Rank
        [Authorize(Roles = "HR")]
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

            var spec = new ApplicationsWithJobSpec(company.Id);
            var applications = await _applicationRepo.GetAllWithSpecAsync(spec);

            var topApplications = applications
                .Where(a => a.RankScore > 0)
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
        #endregion

        #region Get Most Vs Least Applied Jobs
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
            var posts = await _postRepo.GetAllWithSpecAsync(spec);

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
        #endregion

        #region Get AI Screening Summary
        [Authorize(Roles = "HR,Manager")]
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

            var jobSpec = new JobWithCompanySpec(jobId);
            var job = await _postRepo.GetByEntityWithSpecAsync(jobSpec);

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
        #endregion

        #endregion

        #region Ali

        #region Get Companies Count Report
        [Authorize(Roles = "Admin")]
        [HttpGet("system/companies-count")]
        public async Task<ActionResult<CompanyCountReportDto>> GetCompaniesCountReport(DateTime fromDate, DateTime toDate)
        {
            var spec = new AllCompaniesSpec(fromDate, toDate);
            var companies = await _companyRepo.GetAllWithSpecAsync(spec);

            var currentYear = DateTime.UtcNow.Year;
            var fromYear = currentYear - 3;

            var grouped = companies
                .Where(c => c.CreatedAt.Year >= fromYear)
                .GroupBy(c => c.CreatedAt.Year)
                .ToDictionary(g => g.Key, g => g.Count());

            var companiesPerYear = new Dictionary<int, int>();
            for (int year = fromYear; year <= currentYear; year++)
            {
                companiesPerYear[year] = grouped.ContainsKey(year) ? grouped[year] : 0;
            }

            var report = new CompanyCountReportDto
            {
                TotalCompanies = companies.Count(),
                Companies = companies.Select(c => new CompanyDto
                {
                    Id = c.Id,
                    Name = c.Name
                }).ToList(),
                CompaniesPerYear = companiesPerYear
            };

            return Ok(report);
        }
        #endregion

        #region Get Agencies Count Report
        [Authorize(Roles = "Admin")]
        [HttpGet("system/agencies-count")]
        public async Task<ActionResult<AgencyCountReportDto>> GetAgenciesCountReport(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var spec = new AllAgenciesSpec();
            var query = _userManager.Users.Where(spec.Criteria);

            if (fromDate.HasValue)
                query = query.Where(a => a.CreatedAt >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(a => a.CreatedAt <= toDate.Value);

            var agencies = await query.ToListAsync();

            var currentYear = DateTime.UtcNow.Year;
            var fromYear = currentYear - 3;

            var grouped = agencies
                .Where(a => a.CreatedAt.Year >= fromYear)
                .GroupBy(a => a.CreatedAt.Year)
                .ToDictionary(g => g.Key, g => g.Count());

            var agenciesPerYear = new Dictionary<int, int>();
            for (int year = fromYear; year <= currentYear; year++)
            {
                agenciesPerYear[year] = grouped.ContainsKey(year) ? grouped[year] : 0;
            }

            var report = new AgencyCountReportDto
            {
                TotalAgencies = agencies.Count,
                AgenciesData = agencies.Select(a => new AgencyInfoDto
                {
                    Id = a.Id,
                    AgencyName = a.AgencyName
                }).ToList(),
                AgenciesPerYear = agenciesPerYear
            };

            return Ok(report);
        }
        #endregion


        #region Get Agency Count By Company
        [Authorize(Roles = "Admin,Manager,HR")]
        [HttpGet("companies/{companyId}/agencies-count")]
        public async Task<ActionResult<AgencyyCountReportDto>> GetAgencyCountByCompany(int companyId, DateTime fromDate, DateTime toDate)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized(new ApiResponse(401, "User email not found in token"));

            var user = await _userManager.Users
                .Include(u => u.HRCompany)
                .Include(u => u.ManagedCompany)
                .FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
                return Unauthorized(new ApiResponse(401, "User not found"));

            var userCompany = user.HRCompany ?? user.ManagedCompany;
            var userRoles = await _userManager.GetRolesAsync(user);

            if (!userRoles.Contains("Admin") && (userCompany == null || userCompany.Id != companyId))
                return Forbid();

            var postSpec = new PostsByCompanyIdSpec(companyId);
            var posts = await _postRepo.GetAllWithSpecAsync(postSpec);
            var postIds = posts.Select(p => p.Id).ToList();

            if (!postIds.Any())
            {
                return Ok(new AgencyyCountReportDto
                {
                    CompanyId = companyId,
                    TotalAgencies = 0,
                    Agencies = new List<AgencyyApplicationStatsDto>(),
                    FromDate = fromDate,  // إضافة fromDate في الـ DTO
                    ToDate = toDate      // إضافة toDate في الـ DTO
                });
            }

            var applications = await _applicationRepo.GetAllAsync(a => postIds.Contains(a.PostId)
            && a.AgencyId != null
            && a.ApplicationDate >= fromDate
            && a.ApplicationDate <= toDate);



            var agencyIds = applications.Select(a => a.AgencyId).Distinct().ToList();

            var agencies = await _userManager.Users
                .Where(u => agencyIds.Contains(u.Id))
                .ToListAsync();

            var agencyStats = applications
                .GroupBy(a => a.AgencyId)
                .Select(g =>
                {
                    var agency = agencies.FirstOrDefault(a => a.Id == g.Key);
                    return new AgencyyApplicationStatsDto
                    {
                        AgencyId = g.Key,
                        AgencyName = agency?.AgencyName ?? agency?.DisplayName ?? "Unknown",
                        TotalApplications = g.Count()
                    };
                })
                .ToList();

            var report = new AgencyyCountReportDto
            {
                CompanyId = companyId,
                TotalAgencies = agencyStats.Count(),
                Agencies = agencyStats,
                FromDate = fromDate,  // إضافة fromDate في الـ DTO
                ToDate = toDate      // إضافة toDate في الـ DTO
            };

            return Ok(report);
        }

        #endregion

        #region Get Applications Breakdown Per Agency
        [Authorize(Roles = "Admin,Manager,HR")]
        [HttpGet("agencies/applications-breakdown")]
        public async Task<ActionResult<AgencyApplicationsBreakdownReportDto>> GetApplicationsBreakdownPerAgency(
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized(new ApiResponse(401, "User email not found in token"));

            var user = await _userManager.Users
                .Include(u => u.HRCompany)
                .Include(u => u.ManagedCompany)
                .FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
                return Unauthorized(new ApiResponse(401, "User not found"));

            var userRoles = await _userManager.GetRolesAsync(user);
            var isAdmin = userRoles.Contains("Admin");

            List<Application> applications;
            if (isAdmin)
            {
                var spec = new ApplicationsWithAgencySpec();
                applications = (await _applicationRepo.GetAllWithSpecAsync(spec)).ToList();
            }
            else
            {
                var company = user.HRCompany ?? user.ManagedCompany;
                if (company == null)
                    return Unauthorized(new ApiResponse(401, "User is not associated with a company"));

                var spec = new ApplicationsWithAgencyySpec(company.Id);
                applications = (await _applicationRepo.GetAllWithSpecAsync(spec)).ToList();
            }

            // Apply date filtering using ApplicationDate
            if (fromDate.HasValue)
                applications = applications.Where(a => a.ApplicationDate >= fromDate.Value).ToList();

            if (toDate.HasValue)
                applications = applications.Where(a => a.ApplicationDate <= toDate.Value).ToList();

            var grouped = applications
                .Where(a => a.AgencyId != null)
                .GroupBy(a => new { a.AgencyId, AgencyName = a.Agency != null ? a.Agency.AgencyName : "Unknown" })
                .Select(g => new AgencyApplicationsBreakdownDto
                {
                    AgencyIdString = g.Key.AgencyId,
                    AgencyName = g.Key.AgencyName,
                    ApplicationsCount = g.Count(),
                    Applications = g.Select(a => new ApplicationDetailDto
                    {
                        Id = a.Id,
                        CreatedAt = a.ApplicationDate,
                        FormattedDate = a.ApplicationDate.ToString("yyyy-MM-dd HH:mm:ss")
                    }).ToList()
                })
                .OrderByDescending(x => x.ApplicationsCount)
                .ToList();

            var totalApplications = grouped.Sum(x => x.ApplicationsCount);

            var result = new AgencyApplicationsBreakdownReportDto
            {
                TotalApplications = totalApplications,
                Breakdown = grouped,
                FromDate = fromDate?.ToString("yyyy-MM-dd HH:mm:ss"),
                ToDate = toDate?.ToString("yyyy-MM-dd HH:mm:ss")
            };

            return Ok(result);
        }
        #endregion



        #region Get Paid Jobs Created Report 
        [Authorize(Roles = "Admin,Manager,HR")]
        [HttpGet("jobs/created")]
        public async Task<ActionResult<PaidJobsCountReportDto>> GetPaidJobsCreatedReport(DateTime fromDate, DateTime toDate)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized(new ApiResponse(401, "User email not found in token"));

            var user = await _userManager.Users
                .Include(u => u.HRCompany)
                .Include(u => u.ManagedCompany)
                .FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
                return Unauthorized(new ApiResponse(401, "User not found"));

            var userRoles = await _userManager.GetRolesAsync(user);
            List<Post> paidPosts;

            if (userRoles.Contains("Admin"))
            {
                var spec = new PaidJobsByCompanySpec(fromDate, toDate); // Pass dates to the spec
                paidPosts = (await _postRepo.GetAllWithSpecAsync(spec)).ToList();
            }
            else
            {
                var company = user.HRCompany ?? user.ManagedCompany;
                if (company == null)
                    return Unauthorized(new ApiResponse(401, "User is not associated with a company"));

                var spec = new PaidJobsByCompanySpec(company.Id, fromDate, toDate); // Pass dates to the spec
                paidPosts = (await _postRepo.GetAllWithSpecAsync(spec)).ToList();
            }

            var report = new PaidJobsCountReportDto
            {
                TotalPaidJobs = paidPosts.Count,
                Jobs = paidPosts.Select(p => new PaidJobInfoDto
                {
                    JobId = p.Id,
                    JobName = p.JobTitle
                }).ToList(),
                FromDate = fromDate,  // إضافة التاريخ إلى النتيجة
                ToDate = toDate       // إضافة التاريخ إلى النتيجة
            };

            return Ok(report);
        }
        #endregion


        #region Get Applications Count Per Job 
        [Authorize(Roles = "HR,Manager,Agency")]
        [HttpGet("jobs/applications-count")]
        public async Task<ActionResult<object>> GetApplicationsCountPerJob(DateTime fromDate, DateTime toDate)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized(new ApiResponse(401, "User email not found in token"));

            var user = await _userManager.Users
                .Include(u => u.HRCompany)
                .Include(u => u.ManagedCompany)
                .FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
                return Unauthorized(new ApiResponse(401, "User not found"));

            var userRoles = await _userManager.GetRolesAsync(user);
            IEnumerable<Post> posts;

            if (userRoles.Contains("Agency"))
            {
                // Filter applications by ApplicationDate
                var applications = await _applicationRepo.GetAllAsync(a => a.AgencyId == user.Id && a.ApplicationDate >= fromDate && a.ApplicationDate <= toDate);
                var postIds = applications.Select(a => a.PostId).Distinct().ToList();

                if (!postIds.Any())
                    return Ok(new
                    {
                        Title = "Jobs, Total Application",
                        FromDate = fromDate.ToString("yyyy-MM-dd"),
                        ToDate = toDate.ToString("yyyy-MM-dd"),
                        Year = DateTime.Now.Year,
                        Total = 0,
                        Jobs = new List<JobApplicationsCountDto>()
                    });

                var spec = new PostsWithApplicationsSpec(postIds);
                posts = await _postRepo.GetAllWithSpecAsync(spec);
            }
            else
            {
                var userCompany = user.HRCompany ?? user.ManagedCompany;
                if (userCompany == null)
                    return Unauthorized(new ApiResponse(401, "User is not associated with a company"));

                var spec = new PostsWithApplicationsSpec(userCompany.Id);
                posts = await _postRepo.GetAllWithSpecAsync(spec);
            }

            var jobStats = _mapper.Map<List<JobApplicationsCountDto>>(posts);

            var response = new
            {
                Title = "Jobs, Total Application",
                FromDate = fromDate.ToString("yyyy-MM-dd"),
                ToDate = toDate.ToString("yyyy-MM-dd"),
                Year = DateTime.Now.Year,
                Total = jobStats.Sum(j => j.JobAppliedNumber),
                Jobs = jobStats
            };

            return Ok(response);
        }
        #endregion



        #region Get Pending Interview Summary  
        [Authorize(Roles = "HR,Manager")]
        [HttpGet("interviews/pending-summary")]
        public async Task<ActionResult<PendingInterviewSummaryDto>> GetPendingInterviewSummary(DateTime fromDate, DateTime toDate)
        {
            // Retrieve the user's email from the token  
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized(new ApiResponse(401, "User email not found in token"));

            // Retrieve the user from the database  
            var user = await _userManager.Users
                .Include(u => u.HRCompany)
                .Include(u => u.ManagedCompany)
                .FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
                return Unauthorized(new ApiResponse(401, "User not found"));

            // Determine the user's company (either HRCompany or ManagedCompany)  
            var userCompany = user.HRCompany ?? user.ManagedCompany;

            if (userCompany == null)
                return Unauthorized(new ApiResponse(401, "User is not associated with a company"));

            // Create the specification for filtering interviews based on the date range and company  
            var spec = new InterviewsWithApplicantsSpec(fromDate, toDate, userCompany.Id);

            // Retrieve interviews matching the specification  
            var interviews = await _interviewRepo.GetAllWithSpecAsync(spec);

            // Calculate the total number of interviews  
            var totalInterviews = interviews.Count();

            // Filter out the pending interviews  
            var pendingInterviewsList = interviews
                .Where(i => i.InterviewStatus == InterviewStatus.Pending)
                .Select(i => new PendingInterviewDto
                {
                    JobInterviewName = i.Post.JobTitle,
                    Date = i.Date
                }).ToList();

            // Create the summary DTO with the added date range information  
            var result = new PendingInterviewSummaryDto
            {
                FromDate = fromDate, // Add the fromDate parameter to the DTO  
                ToDate = toDate,     // Add the toDate parameter to the DTO  
                TotalInterviews = totalInterviews,
                TotalPendingInterviews = pendingInterviewsList.Count,
                PendingInterviews = pendingInterviewsList
            };

            return Ok(result);
        }
        #endregion


        #region Get Closed Jobs Count
        [Authorize(Roles = "HR,Manager")]
        [HttpGet("jobs/closed-count")]
        public async Task<ActionResult<JobClosedCountReportDto>> GetClosedJobsCount(
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized(new ApiResponse(401, "User email not found in token"));

            var user = await _userManager.Users
                .Include(u => u.HRCompany)
                .Include(u => u.ManagedCompany)
                .FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
                return Unauthorized(new ApiResponse(401, "User not found"));

            var userCompany = user.HRCompany ?? user.ManagedCompany;
            if (userCompany == null)
                return Unauthorized(new ApiResponse(401, "User is not associated with a company"));

            var spec = new PostsByCompanySpec(userCompany.Id);
            var allCompanyJobs = await _postRepo.GetAllWithSpecAsync(spec);

            var filteredJobs = allCompanyJobs.AsQueryable();

            // Use PostDate for date filtering
            if (fromDate.HasValue)
                filteredJobs = filteredJobs.Where(j => j.PostDate >= fromDate.Value);
            if (toDate.HasValue)
                filteredJobs = filteredJobs.Where(j => j.PostDate <= toDate.Value);

            var closedJobs = filteredJobs
                .Where(j => j.JobStatus == "Closed")
                .Select(j => new ClosedJobDto
                {
                    Id = j.Id,
                    JobName = j.JobTitle,
                    Status = j.JobStatus
                })
                .ToList();

            var report = new JobClosedCountReportDto
            {
                TotalJobs = filteredJobs.Count(),
                JobsClosed = closedJobs,
                TotalClosedJobs = closedJobs.Count,
                FromDate = fromDate,
                ToDate = toDate
            };

            return Ok(report);
        }
        #endregion


        #endregion

    }
}