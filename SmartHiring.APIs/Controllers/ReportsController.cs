using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartHiring.APIs.DTOs;
using SmartHiring.APIs.Errors;
using SmartHiring.Core.Entities.Identity;
using SmartHiring.Core.Entities;
using SmartHiring.Core.Specifications;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SmartHiring.Core;

namespace SmartHiring.APIs.Controllers
{
    public class ReportsController : APIBaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public ReportsController(IUnitOfWork unitOfWork,
            UserManager<AppUser> userManager,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
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
            var filteredInterviews = await _unitOfWork.Repository<Interview>().GetAllWithSpecAsync(spec);

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

        #region Get Top Agencies By Applications
        [Authorize(Roles = "Admin,Manager")]
        [HttpGet("top-agencies-by-applications")]
        public async Task<ActionResult<TopAgencyDto>> GetTopAgenciesByApplications(DateTime fromDate, DateTime toDate)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized(new ApiResponse(401, "User email not found"));

            var user = await _userManager.Users
                .Include(u => u.ManagedCompany)
                .FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
                return Unauthorized(new ApiResponse(401, "User not found"));

            var isAdmin = User.IsInRole("Admin");
            int? companyId = isAdmin ? null : user.ManagedCompany?.Id;

            var applicationRepo = _unitOfWork.Repository<Application>();

            if (isAdmin)
            {
                var spec = new ApplicationsWithPostAndAgencySpec(null, fromDate, toDate);
                var allApplications = await applicationRepo.GetAllWithSpecAsync(spec);

                var grouped = allApplications
                    .Where(a => a.Agency != null)
                    .GroupBy(a => a.AgencyId)
                    .Select(g => new TopAgencyItemDto
                    {
                        Name = g.First().Agency.DisplayName,
                        JobAppliedNumber = g.Count()
                    })
                    .OrderByDescending(x => x.JobAppliedNumber)
                    .ToList();

                return Ok(new TopAgencyDto
                {
                    TotalAgencies = grouped.Count,
                    TopAgencies = grouped
                });
            }

            if (companyId == null)
                return Unauthorized(new ApiResponse(401, "Manager is not associated with a company"));

            var companySpec = new ApplicationsWithPostAndAgencySpec(companyId, fromDate, toDate);
            var companyApplications = await applicationRepo.GetAllWithSpecAsync(companySpec);

            var companyGrouped = companyApplications
                .Where(a => a.Agency != null)
                .GroupBy(a => a.AgencyId)
                .Select(g => new TopAgencyItemDto
                {
                    Name = g.First().Agency.DisplayName,
                    JobAppliedNumber = g.Count()
                })
                .OrderByDescending(x => x.JobAppliedNumber)
                .ToList();

            return Ok(new TopAgencyDto
            {
                TotalAgencies = companyGrouped.Count,
                TopAgencies = companyGrouped
            });
        }
        #endregion

        #region Get Jobs Fill Status Report
        [Authorize(Roles = "HR,Manager")]
        [HttpGet("jobs/fill-status")]
        public async Task<ActionResult<JobFillStatusReportDto>> GetJobsFillStatusReport(DateTime fromDate, DateTime toDate)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized(new ApiResponse(401, "User email not found"));

            var user = await _userManager.Users
                .Include(u => u.HRCompany)
                .Include(u => u.ManagedCompany)
                .FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
                return Unauthorized(new ApiResponse(401, "User not found"));

            var company = user.HRCompany ?? user.ManagedCompany;
            if (company == null)
                return Unauthorized(new ApiResponse(401, "User is not associated with a company"));

            var spec = new JobsWithApplicationsSpec(company.Id, fromDate, toDate);
            var posts = await _unitOfWork.Repository<Post>().GetAllWithSpecAsync(spec);

            var filledPosts = posts
                .Where(p => p.Applications.Any())
                .Select(p => p.JobTitle)
                .ToList();

            var unfilledPosts = posts
                .Where(p => !p.Applications.Any())
                .Select(p => p.JobTitle)
                .ToList();

            var report = new JobFillStatusReportDto
            {
                TotalPosts = posts.Count(),
                FilledPosts = filledPosts.Count,
                UnfilledPosts = unfilledPosts.Count,
                FilledPostsData = filledPosts,
                UnfilledPostsData = unfilledPosts
            };

            return Ok(report);
        }
        #endregion

        #region Get AI Screening Ratio

        [Authorize(Roles = "Admin,HR")]
        [HttpGet("ai/acceptance-rejection-ratio")]
        public async Task<ActionResult<AIScreeningReportDto>> GetAIScreeningRatio(
            [FromQuery] int? jobId = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized(new ApiResponse(401, "User email not found"));

            var user = await _userManager.Users
                .Include(u => u.HRCompany)
                .FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
                return Unauthorized(new ApiResponse(401, "User not found"));

            List<Application> applications;

            if (User.IsInRole("HR"))
            {
                if (jobId == null)
                    return BadRequest(new ApiResponse(400, "Job ID is required for HR users"));

                var jobSpec = new JobWithCompanySpec(jobId.Value);
                var job = await _unitOfWork.Repository<Post>().GetByEntityWithSpecAsync(jobSpec);

                if (job == null || job.CompanyId != user.HRCompany?.Id)
                    return Forbid();

                var spec = new ApplicationsForJobSpec(jobId.Value);
                applications = (await _unitOfWork.Repository<Application>().GetAllWithSpecAsync(spec)).ToList();
            }
            else // Admin
            {
                var spec = new ApplicationsWithPostSpec();
                var allApps = await _unitOfWork.Repository<Application>().GetAllWithSpecAsync(spec);

                applications = allApps
                    .Where(a =>
                        (!fromDate.HasValue || a.ApplicationDate.Date >= fromDate.Value.Date) &&
                        (!toDate.HasValue || a.ApplicationDate.Date <= toDate.Value.Date))
                    .ToList();
            }

            var accepted = applications.Count(a => a.IsShortlisted == true);
            var rejected = applications.Count(a => !a.IsShortlisted == true);
            var total = accepted + rejected;

            double acceptanceRatio = total == 0 ? 0 : (double)accepted / total;
            double rejectionRatio = total == 0 ? 0 : (double)rejected / total;

            var result = new AIScreeningReportDto
            {
                AcceptanceRatioByAI = Math.Round(acceptanceRatio * 100, 2),
                RejectionRatioByAI = Math.Round(rejectionRatio * 100, 2),
            };

            return Ok(result);
        }
        #endregion

        #region Get Interview Success Rate
        [Authorize(Roles = "HR")]
        [HttpGet("interview/success-rate/{jobId}")]
        public async Task<ActionResult<InterviewSuccessRateDto>> GetInterviewSuccessRate(int jobId, DateTime fromDate, DateTime toDate)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized(new ApiResponse(401, "User email not found"));

            var user = await _userManager.Users
                .Include(u => u.HRCompany)
                .FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
                return Unauthorized(new ApiResponse(401, "User not found"));

            if (user.HRCompany == null)
            {
                user.HRCompany = new Company { Id = 20, Name = "Nexus Soft" };
            }

            var jobSpec = new JobWithCompanySpec(jobId);
            var job = await _unitOfWork.Repository<Post>().GetByEntityWithSpecAsync(jobSpec);

            if (job == null || job.CompanyId != user.HRCompany.Id)
                return Forbid();

            var spec = new InterviewsForJobSpec(jobId, fromDate, toDate);
            var interviews = await _unitOfWork.Repository<Interview>().GetAllWithSpecAsync(spec);

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
        [HttpGet("applications")]
        public async Task<ActionResult<PostApplicationsDto>> GetPostApplications(DateTime fromDate, DateTime toDate)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized(new ApiResponse(401, "User email not found"));

            var user = await _userManager.Users
                .Include(u => u.ManagedCompany)
                .Include(u => u.HRCompany)
                .FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
                return Unauthorized(new ApiResponse(401, "User not found"));

            var userCompanyId = user.ManagedCompany?.Id ?? user.HRCompany?.Id;

            if (userCompanyId == null)
                return Forbid();

            var spec = new PostsWithApplicationsSpec(userCompanyId.Value, fromDate, toDate);
            var posts = await _unitOfWork.Repository<Post>().GetAllWithSpecAsync(spec);

            var jobData = posts.Select(p => new JobApplicationDataDto
            {
                JobName = p.JobTitle,
                ApplicationNumber = p.Applications?.Count() ?? 0
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
        public async Task<ActionResult<RankedApplicationsReportDto>> GetTopApplicationsByRank(DateTime fromDate, DateTime toDate)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized(new ApiResponse(401, "User email not found"));

            var user = await _userManager.Users
                .Include(u => u.HRCompany)
                .FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
                return Unauthorized(new ApiResponse(401, "User not found"));

            var company = user.HRCompany;
            if (company == null)
                return Unauthorized(new ApiResponse(401, "User is not associated with a company"));

            var spec = new ApplicationsWithJobSpec(company.Id, fromDate, toDate);
            var applications = await _unitOfWork.Repository<Application>().GetAllWithSpecAsync(spec);

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
        public async Task<ActionResult<JobApplicationComparisonDto>> GetMostVsLeastAppliedJobs(DateTime fromDate, DateTime toDate)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized(new ApiResponse(401, "User email not found"));

            var user = await _userManager.Users
                .Include(u => u.ManagedCompany)
                .Include(u => u.HRCompany)
                .FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
                return Unauthorized(new ApiResponse(401, "User not found"));

            var companyId = user.ManagedCompany?.Id ?? user.HRCompany?.Id;
            if (companyId == null)
                return Forbid();

            var spec = new PostsWithApplicationsSpec(companyId.Value, fromDate, toDate);
            var posts = await _unitOfWork.Repository<Post>().GetAllWithSpecAsync(spec);

            var jobsWithCounts = posts
                .Where(p => p.Applications != null && p.Applications.Count > 0)
                .Select(p => new
                {
                    Title = p.JobTitle,
                    Count = p.Applications.Count
                })
                .ToList();

            if (!jobsWithCounts.Any())
            {
                return Ok(new JobApplicationComparisonDto
                {
                    TotalJobsApplied = 0,
                    CandidatesData = new CandidatesDataDto
                    {
                        MostJobTitle = new List<string> { "N/A" },
                        LeastJobTitle = new List<string> { "N/A" }
                    }
                });
            }

            var maxCount = jobsWithCounts.Max(j => j.Count);
            var minCount = jobsWithCounts.Min(j => j.Count);

            var mostJobTitles = jobsWithCounts
                .Where(j => j.Count == maxCount)
                .Select(j => j.Title)
                .ToList();

            var leastJobTitles = jobsWithCounts
                .Where(j => j.Count == minCount)
                .Select(j => j.Title)
                .Except(mostJobTitles)
                .ToList();

            return Ok(new JobApplicationComparisonDto
            {
                TotalJobsApplied = jobsWithCounts.Count,
                CandidatesData = new CandidatesDataDto
                {
                    MostJobTitle = mostJobTitles,
                    LeastJobTitle = leastJobTitles.Any() ? leastJobTitles : new List<string> { "N/A" }
                }
            });
        }

        #endregion

        #region Get AI Screening Summary

        [Authorize(Roles = "HR,Manager")]
        [HttpGet("ai-screening/summary/{jobId}")]
        public async Task<ActionResult<AIScreeningSummaryDto>> GetAIScreeningSummary(int jobId)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized(new ApiResponse(401, "User email not found"));

            var user = await _userManager.Users
                .Include(u => u.HRCompany)
                .Include(u => u.ManagedCompany)
                .FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
                return Unauthorized(new ApiResponse(401, "User not found"));

            var userCompanyId = user.HRCompany?.Id ?? user.ManagedCompany?.Id;
            if (userCompanyId == null)
                return Forbid();

            var jobSpec = new JobWithCompanySpec(jobId);
            var job = await _unitOfWork.Repository<Post>().GetByEntityWithSpecAsync(jobSpec);

            if (job == null || job.CompanyId != userCompanyId)
                return Forbid();

            var spec = new ApplicationsWithApplicantSpec(jobId);
            var applications = await _unitOfWork.Repository<Application>().GetAllWithSpecAsync(spec);

            if (!applications.Any())
            {
                return Ok(new AIScreeningSummaryDto
                {
                    AcceptedApplicants = new(),
                    RejectedApplicants = new()
                });
            }

            var accepted = applications
                .Where(a => a.IsShortlisted == true)
                .Select(a => new ApplicantDto
                {
                    FullName = $"{a.Applicant.FName} {a.Applicant.LName}",
                    Email = a.Applicant.Email
                })
                .ToList();

            var rejected = applications
                .Where(a => !a.IsShortlisted == true)
                .Select(a => new ApplicantDto
                {
                    FullName = $"{a.Applicant.FName} {a.Applicant.LName}",
                    Email = a.Applicant.Email
                })
                .ToList();

            var result = new AIScreeningSummaryDto
            {
                AcceptedApplicants = accepted,
                RejectedApplicants = rejected
            };

            return Ok(result);
        }

        #endregion

        #region Get Company Post Status

        [Authorize(Roles = "Admin")]
        [HttpGet("company-post-status")]
        public async Task<IActionResult> GetCompanyPostStatus([FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            var spec = new CompaniesWithPostsSpec();
            var companies = await _unitOfWork.Repository<Company>().GetAllWithSpecAsync(spec);

            var filteredCompanies = companies
                .Select(c => new
                {
                    Company = c,
                    PostsInRange = c.Posts.Where(p => p.PostDate >= fromDate && p.PostDate <= toDate).ToList()
                })
                .ToList();

            var dto = new CompanyPostStatsDto
            {
                TotalCompanies = filteredCompanies.Count,
                TotalPosts = filteredCompanies.Sum(c => c.PostsInRange.Count),
                PostsPerCompany = filteredCompanies.Select(c => new CompanyPostCountDto
                {
                    CompanyName = c.Company.Name,
                    TotalPosts = c.PostsInRange.Count
                }).ToList()
            };

            return Ok(dto);
        }
        #endregion

        #region Get Applicant Status
        [Authorize(Roles = "HR,Manager")]
        [HttpGet("applicant-status")]
        public async Task<IActionResult> GetApplicantStatus([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var currentUser = await _userManager.FindByEmailAsync(userEmail);

            string hrId = null;
            int? companyId = null;

            if (User.IsInRole("HR"))
            {
                hrId = currentUser.Id;
            }
            else if (User.IsInRole("Manager"))
            {
                companyId = currentUser.HRCompany?.Id;
            }
            var spec = new ApplicantsWithAgencySpec(hrId, companyId, fromDate, toDate);
            var applications = await _unitOfWork.Repository<Application>().GetAllWithSpecAsync(spec);

            var distinctApplicants = applications
                .GroupBy(a => a.ApplicantId)
                .Select(g => g.First())
                .ToList();

            var dto = new ApplicantStatusDto
            {
                TotalApplicants = distinctApplicants.Count,
                ApplicantData = distinctApplicants.Select(a => new ApplicantInfoDto
                {
                    ApplicantName = $"{a.Applicant.FName} {a.Applicant.LName}",
                    AgencyName = a.Agency?.DisplayName ?? "N/A",
                    Phone = a.Applicant.Phone
                }).ToList()
            };

            return Ok(dto);
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
            var companies = await _unitOfWork.Repository<Company>().GetAllWithSpecAsync(spec);

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
                    BusinessEmail = c.BusinessEmail,
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
            var allUsers = await _userManager.GetUsersInRoleAsync("Agency");

            if (fromDate.HasValue)
                allUsers = allUsers.Where(a => a.CreatedAt >= fromDate.Value).ToList();

            if (toDate.HasValue)
                allUsers = allUsers.Where(a => a.CreatedAt <= toDate.Value).ToList();

            var currentYear = DateTime.UtcNow.Year;
            var fromYear = currentYear - 3;

            var grouped = allUsers
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
                TotalAgencies = allUsers.Count,
                AgenciesData = allUsers.Select(a => new AgencyInfoDto
                {
                    AgencyName = a.AgencyName
                }).ToList(),
                AgenciesPerYear = agenciesPerYear
            };

            return Ok(report);
        }
        #endregion

        #region Get Agency Count By Company
        [Authorize(Roles = "Admin,Manager,HR")]
        [HttpGet("agencies-count")]
        public async Task<ActionResult<AgencyyCountReportDto>> GetAgencyCountByCompany(DateTime fromDate, DateTime toDate)
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
                applications = (await _unitOfWork.Repository<Application>().GetAllAsync(a =>
                    a.AgencyId != null &&
                    a.ApplicationDate >= fromDate &&
                    a.ApplicationDate <= toDate
                )).ToList();
            }
            else
            {
                var userCompany = user.HRCompany ?? user.ManagedCompany;
                if (userCompany == null)
                    return Forbid();

                var companyPostIds = (await _unitOfWork.Repository<Post>()
                    .GetAllAsync(p => p.CompanyId == userCompany.Id))
                    .Select(p => p.Id)
                    .ToList();

                applications = (await _unitOfWork.Repository<Application>().GetAllAsync(a =>
                    a.AgencyId != null &&
                    companyPostIds.Contains(a.PostId) &&
                    a.ApplicationDate >= fromDate &&
                    a.ApplicationDate <= toDate
                )).ToList();
            }
            var agencyIds = applications.Select(a => a.AgencyId).Distinct().ToList();

            var allUsers = await _userManager.Users.ToListAsync();
            var agencyUsers = new List<AppUser>();
            foreach (var u in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(u);
                if (roles.Contains("Agency") && (isAdmin || agencyIds.Contains(u.Id)))
                {
                    agencyUsers.Add(u);
                }
            }

            var agencyStats = agencyUsers.Select(agency =>
            {
                var agencyApps = applications.Where(a => a.AgencyId == agency.Id).ToList();
                return new AgencyyApplicationStatusDto
                {
                    AgencyName = agency.AgencyName ?? agency.DisplayName ?? "Unknown",
                    TotalApplications = agencyApps.Count
                };
            }).ToList();

            var report = new AgencyyCountReportDto
            {
                TotalAgencies = agencyStats.Count,
                Agencies = agencyStats
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
                applications = (await _unitOfWork.Repository<Application>().GetAllWithSpecAsync(spec)).ToList();
            }
            else
            {
                var company = user.HRCompany ?? user.ManagedCompany;
                if (company == null)
                    return Unauthorized(new ApiResponse(401, "User is not associated with a company"));

                var spec = new ApplicationsWithAgencyySpec(company.Id);
                applications = (await _unitOfWork.Repository<Application>().GetAllWithSpecAsync(spec)).ToList();
            }

            if (fromDate.HasValue)
                applications = applications.Where(a => a.ApplicationDate >= fromDate.Value).ToList();

            if (toDate.HasValue)
                applications = applications.Where(a => a.ApplicationDate <= toDate.Value).ToList();

            var grouped = applications
                .Where(a => a.AgencyId != null)
                .GroupBy(a => new { a.AgencyId, AgencyName = a.Agency != null ? a.Agency.AgencyName : "Unknown" })
                .Select(g => new AgencyApplicationsBreakdownDto
                {
                    AgencyName = g.Key.AgencyName,
                    ApplicationsCount = g.Count(),
                    Applications = g.Select(a => new ApplicationDetailDto
                    {
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
                var spec = new PaidJobsByCompanySpec(fromDate, toDate);
                paidPosts = (await _unitOfWork.Repository<Post>().GetAllWithSpecAsync(spec)).ToList();
            }
            else
            {
                var company = user.HRCompany ?? user.ManagedCompany;
                if (company == null)
                    return Unauthorized(new ApiResponse(401, "User is not associated with a company"));

                var spec = new PaidJobsByCompanySpec(company.Id, fromDate, toDate);
                paidPosts = (await _unitOfWork.Repository<Post>().GetAllWithSpecAsync(spec)).ToList();
            }

            var report = new PaidJobsCountReportDto
            {
                TotalPaidJobs = paidPosts.Count,
                Jobs = paidPosts.Select(p => new PaidJobInfoDto
                {
                    City = p.City,
                    Requirements = p.Requirements,
                    JobName = p.JobTitle
                }).ToList()
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
            List<Post> posts;

            if (userRoles.Contains("Agency"))
            {
                var agencyApplications = await _unitOfWork.Repository<Application>().GetAllAsync(a =>
                    a.AgencyId == user.Id &&
                    a.ApplicationDate >= fromDate && a.ApplicationDate <= toDate);

                var postIds = agencyApplications.Select(a => a.PostId).Distinct().ToList();

                var spec = new PostsWithApplicationsSpec(p => postIds.Contains(p.Id));
                posts = (await _unitOfWork.Repository<Post>().GetAllWithSpecAsync(spec)).ToList();

                var jobStats = posts.Select(post => new JobApplicationsCountDto
                {
                    Name = post.JobTitle,
                    JobAppliedNumber = post.Applications.Count(a => a.AgencyId == user.Id &&
                                                                     a.ApplicationDate >= fromDate &&
                                                                     a.ApplicationDate <= toDate)
                }).ToList();

                return Ok(new
                {
                    TotalApplications = jobStats.Sum(j => j.JobAppliedNumber),
                    TotalJob = jobStats.Count,
                    Jobs = jobStats
                });
            }
            else
            {
                var userCompany = user.HRCompany ?? user.ManagedCompany;
                if (userCompany == null)
                    return Unauthorized(new ApiResponse(401, "User is not associated with a company"));

                var spec = new PostsWithApplicationsSpec(p =>
                    p.CompanyId == userCompany.Id &&
                    p.PaymentStatus == "Paid" &&
                    p.PostDate >= fromDate && p.PostDate <= toDate);

                posts = (await _unitOfWork.Repository<Post>().GetAllWithSpecAsync(spec)).ToList();

                var jobStatus = posts.Select(post => new JobApplicationsCountDto
                {
                    Name = post.JobTitle,
                    JobAppliedNumber = post.Applications.Count()
                }).ToList();

                return Ok(new
                {
                    TotalApplications = jobStatus.Sum(j => j.JobAppliedNumber),
                    TotalJob = jobStatus.Count,
                    Jobs = jobStatus
                });
            }
        }

        #endregion

        #region Get Pending Interview Summary  
        [Authorize(Roles = "HR,Manager")]
        [HttpGet("interviews/pending-summary")]
        public async Task<ActionResult<PendingInterviewSummaryDto>> GetPendingInterviewSummary(DateTime fromDate, DateTime toDate)
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

            var spec = new InterviewsWithApplicantsSpec(fromDate, toDate, userCompany.Id);
            var interviews = await _unitOfWork.Repository<Interview>().GetAllWithSpecAsync(spec);

            var totalInterviews = interviews.Count();

            var pendingInterviews = interviews
                .Where(i => i.InterviewStatus == InterviewStatus.Pending)
                .ToList();

            var pendingInterviewsList = pendingInterviews
                .Select(i => new PendingInterviewDto
                {
                    ApplicantName = $"{i.Applicant.FName} {i.Applicant.LName}",
                    Location = i.Location,
                    Date = i.Date,
                    Time = i.Time
                }).ToList();

            var agencyPendingSummary = pendingInterviews
                .SelectMany(i => i.Applicant.Applications
                    .Where(a => a.PostId == i.PostId && a.Agency != null)
                    .Select(a => new
                    {
                        AgencyName = a.Agency.AgencyName,
                        ApplicantId = a.ApplicantId
                    })
                )
                .GroupBy(x => x.AgencyName)
                .Select(g => new AgencyPendingSummaryDto
                {
                    AgencyName = g.Key,
                    PendingApplicantsCount = g.Select(x => x.ApplicantId).Distinct().Count()
                })
                .ToList();

            var result = new PendingInterviewSummaryDto
            {
                TotalInterviews = totalInterviews,
                TotalPendingInterviews = pendingInterviewsList.Count,
                PendingInterviews = pendingInterviewsList,
                AgenciesPendingSummary = agencyPendingSummary
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
            var allCompanyJobs = await _unitOfWork.Repository<Post>().GetAllWithSpecAsync(spec);

            var filteredJobs = allCompanyJobs.AsQueryable();

            if (fromDate.HasValue)
                filteredJobs = filteredJobs.Where(j => j.PostDate >= fromDate.Value);
            if (toDate.HasValue)
                filteredJobs = filteredJobs.Where(j => j.PostDate <= toDate.Value);

            var closedJobs = filteredJobs
                .Where(j => j.Deadline != DateTime.MinValue && j.Deadline < DateTime.UtcNow)
                .Select(j => new ClosedJobDto
                {
                    JobName = j.JobTitle,
                    Status = "Closed"
                })
                .ToList();

            var report = new JobClosedCountReportDto
            {
                TotalJobs = filteredJobs.Count(),
                JobsClosed = closedJobs,
                TotalClosedJobs = closedJobs.Count(),
            };

            return Ok(report);
        }
        #endregion

        #region Get Agency Acceptance Rejection Report
        [Authorize(Roles = "Admin")]
        [HttpGet("agency-acceptance-rejection-report")]
        public async Task<ActionResult<object>> GetAgencyAcceptanceRejectionReport(DateTime fromDate, DateTime toDate)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized(new ApiResponse(401, "User email not found in token"));

            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
                return Unauthorized(new ApiResponse(401, "User not found"));

            var agencyUsers = await _userManager.GetUsersInRoleAsync("Agency");

            if (agencyUsers == null || !agencyUsers.Any())
                return Ok(new { summary = new { totalApplications = 0, accepted = 0, rejected = 0 }, agencyReports = new List<object>() });

            var totalApplications = 0;
            var accepted = 0;
            var rejected = 0;
            var agencyReports = new List<object>();

            foreach (var agency in agencyUsers)
            {
                var spec = new ApplicationByAgencyySpec(agency.Id, fromDate, toDate);
                var applications = await _unitOfWork.Repository<Application>().GetAllWithSpecAsync(spec);

                if (applications == null || !applications.Any())
                    continue;

                var shortlistedCount = applications.Count(a => a.IsShortlisted == true);
                var totalApplicationsForAgency = applications.Count();
                var rejectedCount = totalApplicationsForAgency - shortlistedCount;

                totalApplications += totalApplicationsForAgency;
                accepted += shortlistedCount;
                rejected += rejectedCount;

                var acceptanceRate = totalApplicationsForAgency > 0 ? (double)shortlistedCount / totalApplicationsForAgency * 100 : 0;
                var rejectionRate = totalApplicationsForAgency > 0 ? (double)rejectedCount / totalApplicationsForAgency * 100 : 0;

                var report = new
                {
                    AgencyName = agency.AgencyName,
                    AcceptanceRate = Math.Round(acceptanceRate, 2),
                    RejectionRate = Math.Round(rejectionRate, 2)
                };

                agencyReports.Add(report);
            }

            var summary = new
            {
                TotalApplications = totalApplications,
                Accepted = accepted,
                Rejected = rejected
            };

            var result = new
            {
                Summary = summary,
                AgencyReports = agencyReports
            };

            return Ok(result);
        }
        #endregion

        #region Get Company Acceptance Rejection Report
        [Authorize(Roles = "HR,Manager")]
        [HttpGet("company-acceptance-rejection-report")]
        public async Task<ActionResult<CompanyAcceptanceRejectionReportDto>> GetCompanyAcceptanceRejectionReport(DateTime fromDate, DateTime toDate)
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

            var spec = new ApplicationsByCompanyySpec(userCompany.Id, fromDate, toDate);
            var applications = await _unitOfWork.Repository<Application>().GetAllWithSpecAsync(spec);

            if (!applications.Any())
                return Ok(new CompanyAcceptanceRejectionReportDto
                {
                    TotalApplications = 0,
                    Accepted = 0,
                    Rejected = 0,
                    CompanyReports = new List<CompanyAcceptanceRejectionDto>()
                });

            var total = applications.Count();
            var accepted = applications.Count(a => a.IsShortlisted == true);
            var rejected = total - accepted;

            var companyReport = new CompanyAcceptanceRejectionDto
            {
                CompanyName = userCompany.Name,
                AcceptanceRate = total > 0 ? Math.Round((double)accepted / total * 100, 2) : 0,
                RejectionRate = total > 0 ? Math.Round((double)rejected / total * 100, 2) : 0
            };

            var result = new CompanyAcceptanceRejectionReportDto
            {
                TotalApplications = total,
                Accepted = accepted,
                Rejected = rejected,
                CompanyReports = new List<CompanyAcceptanceRejectionDto> { companyReport }
            };

            return Ok(result);
        }
        #endregion

        #endregion

    }
}