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
        private readonly IGenericRepository<Application> _applicationRepo;
        private readonly IGenericRepository<Post> _postRepo;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public ReportsController(
            IGenericRepository<Interview> interviewRepo,
            IGenericRepository<Company> companyRepo,
            IGenericRepository<Application> applicationRepo,
            IGenericRepository<Post> postRepo,
            UserManager<AppUser> userManager,
            IMapper mapper)
        {
            _interviewRepo = interviewRepo;
            _companyRepo = companyRepo;
            _applicationRepo = applicationRepo;
            _postRepo = postRepo;
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

        #region R1 - عدد الـ Companies
        [Authorize(Roles = "Admin")]
        [HttpGet("system/companies-count")]
        public async Task<ActionResult<int>> GetCompaniesCount()
        {
            var spec = new BaseSpecification<Company>();
            var companies = await _companyRepo.GetAllWithSpecAsync(spec);

            return Ok(companies.Count);
        }
        #endregion

        #region R2 - عدد الـ Agencies
        [Authorize(Roles = "Admin")]
        [HttpGet("system/agencies-count")]
        public async Task<ActionResult<int>> GetAgenciesCount()
        {
            var spec = new UsersByRoleSpecification("Agency");
            var agencies = await _userRepo.GetAllWithSpecAsync(spec);

            return Ok(agencies.Count);
        }
        #endregion

        #region R3 - عدد الـ Agencies اللي قدمت Applications لشركة معينة
        [Authorize(Roles = "Admin,Manager,HR")]
        [HttpGet("companies/{companyId}/agencies-count")]
        public async Task<ActionResult<int>> GetAgenciesCountForCompany(int companyId)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail)) return Unauthorized(new ApiResponse(401, "User email not found"));

            var user = await _agencyRepo.Users
                .Include(u => u.HRCompany)
                .Include(u => u.ManagedCompany)
                .FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null) return Unauthorized(new ApiResponse(401, "User not found"));

            var userCompany = user.HRCompany ?? user.ManagedCompany;

            if (!User.IsInRole("Admin") && (userCompany == null || userCompany.Id != companyId))
                return Unauthorized(new ApiResponse(401, "Not authorized for this company"));

            var spec = new AgenciesByCompanySpecification(companyId);
            var agencies = await _applicationRepo.GetAllWithSpecAsync(spec);

            var distinctAgencies = agencies
                .Select(a => a.AgencyId)
                .Distinct()
                .Count();

            return Ok(distinctAgencies);
        }
        #endregion

        #region R5 - متوسط عدد الـ Applications اللي قدمتها كل Agency
        [Authorize(Roles = "Admin,Manager,HR")]
        [HttpGet("agencies/average-applications")]
        public async Task<ActionResult<double>> GetAverageApplicationsPerAgency()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail)) return Unauthorized(new ApiResponse(401, "User email not found"));

            var user = await _agencyRepo.Users
                .Include(u => u.HRCompany)
                .Include(u => u.ManagedCompany)
                .FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null) return Unauthorized(new ApiResponse(401, "User not found"));

            var userCompany = user.HRCompany ?? user.ManagedCompany;

            BaseSpecification<Application> spec = User.IsInRole("Admin")
                ? new BaseSpecification<Application>()
                : new ApplicationsForCompanySpecification(userCompany.Id);

            var applications = await _applicationRepo.GetAllWithSpecAsync(spec);

            var agencyGroups = applications
                .Where(a => a.AgencyId.HasValue)
                .GroupBy(a => a.AgencyId)
                .ToList();

            var average = agencyGroups.Any() ? agencyGroups.Average(g => g.Count()) : 0;

            return Ok(average);
        }
        #endregion

        #region R6 - عدد الوظائف المدفوعة
        [Authorize(Roles = "Admin,Manager,HR")]
        [HttpGet("jobs/created")]
        public async Task<ActionResult<int>> GetPaidCreatedJobs()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail)) return Unauthorized(new ApiResponse(401, "User email not found"));

            var user = await _agencyRepo.Users
                .Include(u => u.HRCompany)
                .Include(u => u.ManagedCompany)
                .FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null) return Unauthorized(new ApiResponse(401, "User not found"));

            var userCompany = user.HRCompany ?? user.ManagedCompany;

            var spec = User.IsInRole("Admin")
                ? new PaidPostsSpecification()
                : new PaidPostsSpecification(userCompany.Id);

            var posts = await _postRepo.GetAllWithSpecAsync(spec);

            return Ok(posts.Count);
        }
        #endregion

        #region R8 - عدد الـ Applications على كل الوظايف اللي في الشركة
        [Authorize(Roles = "Manager,HR,Agency")]
        [HttpGet("jobs/applications-count")]
        public async Task<ActionResult<List<JobApplicationsCountDto>>> GetApplicationsCountPerJob()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail)) return Unauthorized(new ApiResponse(401, "User email not found"));

            var user = await _agencyRepo.Users
                .Include(u => u.HRCompany)
                .Include(u => u.ManagedCompany)
                .FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null) return Unauthorized(new ApiResponse(401, "User not found"));

            List<Application> applications;
            if (User.IsInRole("Agency"))
            {
                var spec = new ApplicationsByAgencySpecification(user.Id);
                applications = await _applicationRepo.GetAllWithSpecAsync(spec);
            }
            else
            {
                var userCompany = user.HRCompany ?? user.ManagedCompany;
                if (userCompany == null)
                    return Unauthorized(new ApiResponse(401, "User is not associated with a company"));

                var spec = new ApplicationsByCompanyJobsSpecification(userCompany.Id);
                applications = await _applicationRepo.GetAllWithSpecAsync(spec);
            }

            var result = applications
                .GroupBy(a => a.PostId)
                .Select(g => new JobApplicationsCountDto
                {
                    PostId = g.Key,
                    ApplicationsCount = g.Count()
                })
                .ToList();

            return Ok(result);
        }
        #endregion

        #region R11 - عدد الـ Interviews اللي حالتها Pending
        [Authorize(Roles = "Manager,HR")]
        [HttpGet("interviews/pending")]
        public async Task<ActionResult<int>> GetPendingInterviews()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail)) return Unauthorized(new ApiResponse(401, "User email not found"));

            var user = await _agencyRepo.Users
                .Include(u => u.HRCompany)
                .Include(u => u.ManagedCompany)
                .FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null) return Unauthorized(new ApiResponse(401, "User not found"));

            var userCompany = user.HRCompany ?? user.ManagedCompany;
            if (userCompany == null) return Unauthorized(new ApiResponse(401, "User not associated with a company"));

            var spec = new InterviewByStatusSpecification(userCompany.Id, InterviewStatus.Pending);
            var pendingInterviews = await _interviewRepo.GetAllWithSpecAsync(spec);

            return Ok(pendingInterviews.Count);
        }
        #endregion

        #region R15 - عدد الوظائف اللي Closed
        [Authorize(Roles = "Manager,HR")]
        [HttpGet("jobs/closed-count")]
        public async Task<ActionResult<int>> GetClosedJobsCount()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail)) return Unauthorized(new ApiResponse(401, "User email not found"));

            var user = await _agencyRepo.Users
                .Include(u => u.HRCompany)
                .Include(u => u.ManagedCompany)
                .FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null) return Unauthorized(new ApiResponse(401, "User not found"));

            var userCompany = user.HRCompany ?? user.ManagedCompany;
            if (userCompany == null) return Unauthorized(new ApiResponse(401, "User not associated with a company"));

            var spec = new ClosedJobsSpecification(userCompany.Id);
            var closedJobs = await _postRepo.GetAllWithSpecAsync(spec);

            return Ok(closedJobs.Count);
        }
        #endregion

    }
}