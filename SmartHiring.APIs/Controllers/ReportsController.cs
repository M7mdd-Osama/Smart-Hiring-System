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
using System.Linq;
using System.Security.Claims;

namespace SmartHiring.APIs.Controllers
{
    public class ReportsController : APIBaseController
    {
        private readonly IGenericRepository<Interview> _interviewRepo;
        private readonly IGenericRepository<Company> _companyRepo;
        private readonly UserManager<AppUser> _userManager;
        private readonly IGenericRepository<Post> _postRepo;
        private readonly IGenericRepository<Application> _applicationRepo;
        private readonly IMapper _mapper;                     

        public ReportsController(
            IGenericRepository<Interview> interviewRepo,
            IGenericRepository<Company> companyRepo,
            UserManager<AppUser> userManager,
            IGenericRepository<Post> postRepo,
            IGenericRepository<Application> applicationRepo,
            IMapper mapper)
        {
            _interviewRepo = interviewRepo;
            _companyRepo = companyRepo;
            _userManager = userManager;
            _postRepo = postRepo;
            _applicationRepo = applicationRepo;
            _mapper = mapper;
        }

        #region Get Interview Stage Report

        //[Authorize(Roles = "HR,Manager")]
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

        //R1 - عدد الـ Companies اللي موجودين في السيستم
        //[Authorize(Roles = "Admin")]
        [HttpGet("system/companies-count")]
        public async Task<ActionResult<CompanyCountReportDto>> GetCompaniesCountReport()
        {
            var spec = new AllCompaniesSpecification();
            var companies = await _companyRepo.GetAllWithSpecAsync(spec);

            var report = new CompanyCountReportDto
            {
                TotalCompanies = companies.Count(),
                Companies = companies.Select(c => new CompanyDto
                {
                    Id = c.Id,
                    Name = c.Name
                }).ToList()
            };

            return Ok(report);
        }


        //R2 - عدد الـ Agencies اللي في السيستم

        //[Authorize(Roles = "Admin")]
        [HttpGet("system/agencies-count")]
        public async Task<ActionResult<AgencyCountReportDto>> GetAgenciesCountReport()
        {
            var spec = new AllAgenciesSpecification();
            var agencies = await _userManager.Users
                .Where(spec.Criteria)
                .ToListAsync();

            var report = new AgencyCountReportDto
            {
                TotalAgencies = agencies.Count,
                AgenciesData = agencies.Select(a => new AgencyInfoDto
                {
                    Id = a.Id,
                    AgencyName = a.AgencyName
                }).ToList()
            };

            return Ok(report);
        }



        //R3 - عدد الـ Agencies اللي قدمت Applications لشركة معينة

        //[Authorize(Roles = "Admin,Manager,HR")]
        [HttpGet("companies/{companyId}/agencies-count")]
        public async Task<ActionResult<AgencyyCountReportDto>> GetAgencyCountByCompany(int companyId)
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

            // هات كل الـ Posts بتاعة الشركة
            var postSpec = new PostsByCompanyIdSpecification(companyId);
            var posts = await _postRepo.GetAllWithSpecAsync(postSpec);
            var postIds = posts.Select(p => p.Id).ToList();

            if (!postIds.Any())
            {
                return Ok(new AgencyyCountReportDto
                {
                    CompanyId = companyId,
                    TotalAgencies = 0,
                    Agencies = new List<AgencyyApplicationStatsDto>()
                });
            }

            // هات الـ Applications المرتبطة بالـ Posts
            var applications = await _applicationRepo.GetAllAsync(a => postIds.Contains(a.PostId) && a.AgencyId != null);

            // هات الـ Agency Users المرتبطين بالتطبيقات
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
                Agencies = agencyStats
            };

            return Ok(report);
        }



        //R5 - متوسط عدد الـ Applications اللي قدمتها كل Agency

        //[Authorize(Roles = "Admin,Manager,HR")]
        [HttpGet("agencies/applications-breakdown")]
        public async Task<ActionResult<AgencyApplicationsBreakdownReportDto>> GetApplicationsBreakdownPerAgency()
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
                var spec = new ApplicationsWithAgencySpecification();
                applications = (await _applicationRepo.GetAllWithSpecAsync(spec)).ToList();
            }
            else
            {
                var company = user.HRCompany ?? user.ManagedCompany;
                if (company == null)
                    return Unauthorized(new ApiResponse(401, "User is not associated with a company"));

                var spec = new ApplicationsWithAgencySpecification(company.Id);
                applications = (await _applicationRepo.GetAllWithSpecAsync(spec)).ToList();
            }

            var grouped = applications
                .Where(a => a.AgencyId != null)
                .GroupBy(a => new { a.AgencyId, AgencyName = a.Agency != null ? a.Agency.AgencyName : "Unknown" })
                .Select(g => new AgencyApplicationsBreakdownDto
                {
                    AgencyIdString = g.Key.AgencyId,
                    AgencyName = g.Key.AgencyName,
                    ApplicationsCount = g.Count()
                })
                .OrderByDescending(x => x.ApplicationsCount)
                .ToList();

            var totalApplications = grouped.Sum(x => x.ApplicationsCount);

            var result = new AgencyApplicationsBreakdownReportDto
            {
                TotalApplications = totalApplications,
                Breakdown = grouped
            };

            return Ok(result);
        }







        // R6 - عدد الوظائف اللي انشأتها الشركات وبقى الـ PaymentStatus بتاعها "Paid"

        //[Authorize(Roles = "Admin,Manager,HR")]
        [HttpGet("jobs/created")]
        public async Task<ActionResult<PaidJobsCountReportDto>> GetPaidJobsCreatedReport()
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
                var spec = new PaidJobsByCompanySpecification(); // كل الوظائف المدفوعة
                paidPosts = (await _postRepo.GetAllWithSpecAsync(spec)).ToList();
            }
            else
            {
                var company = user.HRCompany ?? user.ManagedCompany;
                if (company == null)
                    return Unauthorized(new ApiResponse(401, "User is not associated with a company"));

                var spec = new PaidJobsByCompanySpecification(company.Id); // وظائف الشركة بس
                paidPosts = (await _postRepo.GetAllWithSpecAsync(spec)).ToList();
            }

            var report = new PaidJobsCountReportDto
            {
                TotalPaidJobs = paidPosts.Count,
                Jobs = paidPosts.Select(p => new PaidJobInfoDto
                {
                    JobId = p.Id,
                    JobName = p.JobTitle // أو .Name حسب التسمية عندك
                }).ToList()
            };

            return Ok(report);
        }








        //R8 - عدد الApplications على كل الوظايف اللي في الشركة.

        //[Authorize(Roles = "HR,Manager,Agency")]
        [HttpGet("jobs/applications-count")]
        public async Task<ActionResult<object>> GetApplicationsCountPerJob()
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
                var applications = await _applicationRepo.GetAllAsync(a => a.AgencyId == user.Id);
                var postIds = applications.Select(a => a.PostId).Distinct().ToList();

                if (!postIds.Any())
                    return Ok(new
                    {
                        Title = "Jobs, Total Application",
                        Total = 0,
                        Jobs = new List<JobApplicationsCountDto>()
                    });

                var spec = new PostsWithApplicationsSpecification(postIds);
                posts = await _postRepo.GetAllWithSpecAsync(spec);
            }
            else
            {
                var userCompany = user.HRCompany ?? user.ManagedCompany;
                if (userCompany == null)
                    return Unauthorized(new ApiResponse(401, "User is not associated with a company"));

                var spec = new PostsWithApplicationsSpecification(userCompany.Id);
                posts = await _postRepo.GetAllWithSpecAsync(spec);
            }

            var jobStats = _mapper.Map<List<JobApplicationsCountDto>>(posts);

            var response = new
            {
                Title = "Jobs, Total Application",
                Total = jobStats.Sum(j => j.JobAppliedNumber),
                Jobs = jobStats
            };

            return Ok(response);
        }




        //R11 - عدد الـ Interviews اللي لسه متعملتش في جدول Interviews لما الInterviewStatus تبقا Pending

        //[Authorize(Roles = "HR,Manager")]
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

            var spec = new InterviewsWithApplicantsSpecification(fromDate, toDate, userCompany.Id);
            var interviews = await _interviewRepo.GetAllWithSpecAsync(spec);

            var totalInterviews = interviews.Count();

            var pendingInterviewsList = interviews
            .Where(i => i.InterviewStatus == InterviewStatus.Pending)
            .Select(i => new PendingInterviewDto
            {
                JobInterviewName = i.Post.JobTitle, // ← أو Name، حسب ما موجود فعليًا
                Date = i.Date
            }).ToList();




            var result = new PendingInterviewSummaryDto
            {
                TotalInterviews = totalInterviews,
                TotalPendingInterviews = pendingInterviewsList.Count,
                PendingInterviews = pendingInterviewsList

            };

            return Ok(result);
        }







        //R15 -  عدد الوظائف اللي خلاص اتقفلت.وبقا الJobStatus بتاعها بقا Closed فالداتا بيز في جدول Posts
        //[Authorize(Roles = "HR,Manager")]
        [HttpGet("jobs/closed-count")]
        public async Task<ActionResult<JobClosedCountReportDto>> GetClosedJobsCount()
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

            // Specification to get all jobs for company
            var spec = new PostsByCompanySpecification(userCompany.Id);
            var allCompanyJobs = await _postRepo.GetAllWithSpecAsync(spec);

            // Filter closed jobs
            var closedJobs = allCompanyJobs
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
                TotalJobs = allCompanyJobs.Count(),  // Invoke Count as a method
                JobsClosed = closedJobs,
                TotalClosedJobs = closedJobs.Count  // Use Count here to get the count of closed jobs
            };

            return Ok(report);
        }







    }
}