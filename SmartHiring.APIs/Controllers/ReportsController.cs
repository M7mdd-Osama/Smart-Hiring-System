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

        #region Ali

        //R1 - عدد الـ Companies اللي موجودين في السيستم
        [Authorize(Roles = "Admin")]
        [HttpGet("system/companies-count")]
        public async Task<ActionResult<CompanyCountReportDto>> GetCompaniesCountReport()
        {
            var spec = new AllCompaniesSpecification();
            var companies = await _companyRepo.GetAllWithSpecAsync(spec);

            var report = new CompanyCountReportDto
            {
                TotalCompanies = companies.Count()
            };

            return Ok(report);
        }

        //R2 - عدد الـ Agencies اللي في السيستم

        [Authorize(Roles = "Admin")]
        [HttpGet("system/agencies-count")]
        public async Task<ActionResult<AgencyCountReportDto>> GetAgenciesCountReport()
        {
            var spec = new AllAgenciesSpecification();
            var agencies = await _userManager.Users
                .Where(spec.Criteria)
                .CountAsync();

            var report = new AgencyCountReportDto
            {
                TotalAgencies = agencies
            };

            return Ok(report);
        }

        //R3 - عدد الـ Agencies اللي قدمت Applications لشركة معينة

        [Authorize(Roles = "Admin,Manager,HR")]
        [HttpGet("companies/{companyId}/agencies-count")]
        public async Task<ActionResult<AgencyCountReportDto>> GetAgencyCountByCompany(int companyId)
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
                return Ok(new AgencyCountReportDto { TotalAgencies = 0 });

            // هات كل الـ Applications اللي مقدمة على الـ Posts دي
            var applications = await _applicationRepo.GetAllAsync(a => postIds.Contains(a.PostId));

            // استخرج عدد الوكالات الفريدة اللي قدمت
            var totalAgencies = applications
                .Where(a => a.AgencyId != null)
                .Select(a => a.AgencyId)
                .Distinct()
                .Count();

            var report = new AgencyCountReportDto
            {
                TotalAgencies = totalAgencies
            };

            return Ok(report);
        }

        //R5 - متوسط عدد الـ Applications اللي قدمتها كل Agency

        //[Authorize(Roles = "Admin,Manager,HR")]
        [HttpGet("agencies/average-applications")]
        public async Task<ActionResult<AgencyApplicationsAvgReportDto>> GetAverageApplicationsPerAgency()
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
                var spec = new ApplicationsWithAgencySpecification(); // هات كل التطبيقات اللي جاية من Agencies
                applications = (await _applicationRepo.GetAllWithSpecAsync(spec)).ToList();
            }
            else
            {
                var company = user.HRCompany ?? user.ManagedCompany;
                if (company == null)
                    return Unauthorized(new ApiResponse(401, "User is not associated with a company"));

                var spec = new ApplicationsWithAgencySpecification(company.Id); // التطبيقات اللي جاية على الشركة دي
                applications = (await _applicationRepo.GetAllWithSpecAsync(spec)).ToList();
            }

            var totalApplications = applications.Count;
            var distinctAgencyCount = applications
                .Where(a => a.AgencyId != null)
                .Select(a => a.AgencyId)
                .Distinct()
                .Count();

            double average = distinctAgencyCount == 0 ? 0 : (double)totalApplications / distinctAgencyCount;

            var report = new AgencyApplicationsAvgReportDto
            {
                AverageApplicationsPerAgency = Math.Round(average, 2)
            };

            return Ok(report);
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
                var spec = new PaidJobsByCompanySpecification(); // الكل
                paidPosts = (await _postRepo.GetAllWithSpecAsync(spec)).ToList(); // تأكدنا من التحويل إلى List
            }
            else
            {
                var company = user.HRCompany ?? user.ManagedCompany;
                if (company == null)
                    return Unauthorized(new ApiResponse(401, "User is not associated with a company"));

                var spec = new PaidJobsByCompanySpecification(company.Id);
                paidPosts = (await _postRepo.GetAllWithSpecAsync(spec)).ToList(); // تأكدنا من التحويل إلى List
            }

            var report = new PaidJobsCountReportDto
            {
                TotalPaidJobs = paidPosts.Count
            };

            return Ok(report);
        }

        //R8 - عدد الApplications على كل الوظايف اللي في الشركة.

        //[Authorize(Roles = "HR,Manager,Agency")]
        [HttpGet("jobs/applications-count")]
        public async Task<ActionResult<IEnumerable<JobApplicationsCountDto>>> GetApplicationsCountPerJob()
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
                // For Agency: jobs that agency has applied to
                var applications = await _applicationRepo.GetAllAsync(a => a.AgencyId == user.Id);
                var postIds = applications.Select(a => a.PostId).Distinct().ToList();

                if (!postIds.Any())
                    return Ok(new List<JobApplicationsCountDto>());

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

            var mapped = _mapper.Map<IEnumerable<JobApplicationsCountDto>>(posts);
            return Ok(mapped);
        }

        //R11 - عدد الـ Interviews اللي لسه متعملتش في جدول Interviews لما الInterviewStatus تبقا Pending

        [Authorize(Roles = "HR,Manager")]
        [HttpGet("interviews/pending")]
        public async Task<ActionResult<PendingInterviewsReportDto>> GetPendingInterviewsReport(DateTime fromDate, DateTime toDate)
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

            var spec = new PendingInterviewsSpecification(fromDate, toDate, userCompany.Id);
            var pendingInterviews = await _interviewRepo.GetAllWithSpecAsync(spec);

            var mapped = _mapper.Map<List<PendingInterviewCandidateDto>>(pendingInterviews);

            var report = new PendingInterviewsReportDto
            {
                TotalPendingInterviews = pendingInterviews.Count(),
                Candidates = mapped
            };

            return Ok(report);
        }

        //R15 -  عدد الوظائف اللي خلاص اتقفلت.وبقا الJobStatus بتاعها بقا Closed فالداتا بيز في جدول Posts

        [Authorize(Roles = "HR,Manager")]
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

            var spec = new JobClosedSpecification(userCompany.Id);
            var closedJobs = await _postRepo.GetAllWithSpecAsync(spec);

            var report = new JobClosedCountReportDto
            {
                TotalClosedJobs = closedJobs.Count()
            };

            return Ok(report);
        }

        #endregion

    }
}