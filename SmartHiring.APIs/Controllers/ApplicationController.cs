using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartHiring.APIs.DTOs;
using SmartHiring.APIs.Errors;
using SmartHiring.APIs.Helpers;
using SmartHiring.Core.Entities;
using SmartHiring.Core.Entities.Identity;
using SmartHiring.Core.Repositories;
using SmartHiring.Core.Specifications;
using System.Security.Claims;

namespace SmartHiring.APIs.Controllers
{
    public class ApplicationController : APIBaseController
    {
        private readonly IGenericRepository<Application> _applicationRepo;
        private readonly IGenericRepository<Applicant> _applicantRepo;
        private readonly IGenericRepository<CandidateListApplicant> _candidateListApplicantRepo;
        private readonly IGenericRepository<CandidateList> _candidateListRepo;
        private readonly IGenericRepository<AgencyApplicant> _agencyApplicantRepo;
        private readonly IGenericRepository<Interview> _interviewRepo;
        private readonly IGenericRepository<Post> _postRepo;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;

        public ApplicationController(
            IGenericRepository<Application> applicationRepo,
            IGenericRepository<Applicant> applicantRepo,
            IGenericRepository<CandidateListApplicant> candidateListApplicantRepo,
            IGenericRepository<CandidateList> candidateListRepo,
            IGenericRepository<AgencyApplicant> agencyApplicantRepo,
            IGenericRepository<Interview> interviewRepo,
            IGenericRepository<Post> postRepo,
            IMapper mapper,
            UserManager<AppUser> userManager)
        {
            _applicationRepo = applicationRepo;
            _applicantRepo = applicantRepo;
            _candidateListApplicantRepo = candidateListApplicantRepo;
            _candidateListRepo = candidateListRepo;
            _agencyApplicantRepo = agencyApplicantRepo;
            _interviewRepo = interviewRepo;
            _postRepo = postRepo;
            _mapper = mapper;
            _userManager = userManager;
        }

        #region Get Applications For Post

        [Authorize(Roles = "HR")]
        [HttpGet("{postId}/applications")]
        public async Task<IActionResult> GetApplicationsForPost(int postId)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized(new ApiResponse(401, "User email not found in token"));

            var user = await _userManager.Users
                .Include(u => u.HRCompany)
                .FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
                return Unauthorized(new ApiResponse(401, "User not found"));

            var post = await _postRepo.GetByIdAsync(postId);

            if (post == null || post.CompanyId != user.HRCompany.Id)
                return Forbid();

            var spec = new ApplicationsByPostIdSpecification(postId);
            var allApplications = await _applicationRepo.GetAllWithSpecAsync(spec);

            if (allApplications == null || !allApplications.Any())
                return NotFound(new ApiResponse(404, "No applications found for this post."));

            var appliedDtos = _mapper.Map<List<ApplicationDto>>(allApplications);

            var filtered = appliedDtos
                .Where(a => allApplications.Any(app => app.Id == a.Id && app.IsShortlisted))
                .ToList();

            var disqualified = appliedDtos
                .Where(a => allApplications.Any(app => app.Id == a.Id && !app.IsShortlisted))
                .ToList();

            var result = new
            {
                Applied = new { Data = appliedDtos, appliedDtos.Count },
                Filtered = new { Data = filtered, filtered.Count },
                Disqualified = new { Data = disqualified, disqualified.Count }
            };

            return Ok(result);
        }

        #endregion

        #region Shortlisted Candidates

        [Authorize(Roles = "HR")]
        [HttpGet("{postId}/ShortlistedCandidates")]
        public async Task<IActionResult> GetFilteredCandidates(int postId)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized(new ApiResponse(401, "User email not found in token"));

            var user = await _userManager.Users
                .Include(u => u.HRCompany)
                .FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
                return Unauthorized(new ApiResponse(401, "User not found"));

            var post = await _postRepo.GetByIdAsync(postId);

            if (post == null || post.CompanyId != user.HRCompany.Id)
                return Forbid();

            var spec = new AcceptedApplicationsByPostIdSpecification(postId);
            var filteredCandidates = await _applicationRepo.GetAllWithSpecAsync(spec);

            if (filteredCandidates == null || !filteredCandidates.Any())
                return NotFound(new ApiResponse(404, "No filtered candidates found for this post."));

            var sortedCandidates = filteredCandidates
                .OrderByDescending(app => app.RankScore)
                .ToList();

            var mappedCandidates = _mapper.Map<List<CandidateForManagerDto>>(sortedCandidates);

            for (int i = 0; i < mappedCandidates.Count; i++)
            {
                mappedCandidates[i].Rank = i + 1;
            }

            var result = new
            {
                TotalCount = mappedCandidates.Count,
                Candidates = mappedCandidates
            };

            return Ok(result);
        }

        #endregion

        #region Create CandidateList For Manager

        [Authorize(Roles = "HR")]
        [HttpPost("{postId}/CreateCandidateList")]
        public async Task<IActionResult> CreateCandidateList(int postId, [FromBody] CandidateListRequestDto request)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized(new ApiResponse(401, "User email not found in token"));

            var user = await _userManager.Users
                .Include(u => u.HRCompany)
                .FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
                return Unauthorized(new ApiResponse(401, "User not found"));

            var post = await _postRepo.GetByIdAsync(postId);
            if (post == null || post.CompanyId != user.HRCompany.Id)
                return Forbid();

            var manager = await _userManager.Users
                .FirstOrDefaultAsync(u => u.ManagedCompany != null && u.ManagedCompany.Id == user.HRCompany.Id);

            if (manager == null)
                return NotFound(new ApiResponse(404, "No manager found for this company."));

            var spec = new AcceptedApplicationsByPostIdSpecification(postId);
            var filteredCandidates = await _applicationRepo.GetAllWithSpecAsync(spec);

            if (filteredCandidates == null || !filteredCandidates.Any())
                return NotFound(new ApiResponse(404, "No filtered candidates found for this post."));

            var topCandidates = filteredCandidates
                .OrderByDescending(app => app.RankScore)
                .Take(request.TopN)
                .ToList();

            if (!topCandidates.Any())
                return BadRequest(new ApiResponse(400, "No candidates available for selection."));

            var candidateList = new CandidateList
            {
                PostId = postId,
                ManagerId = manager.Id,
                Status = "Pending",
                GeneratedDate = DateTime.UtcNow
            };

            await _candidateListRepo.AddAsync(candidateList);
            await _candidateListRepo.SaveChangesAsync();

            var candidateListApplicants = topCandidates.Select(c => new CandidateListApplicant
            {
                CandidateListId = candidateList.Id,
                ApplicantId = c.ApplicantId
            }).ToList();

            await _candidateListApplicantRepo.AddRangeAsync(candidateListApplicants);
            await _candidateListApplicantRepo.SaveChangesAsync();

            return Ok(new { CandidateListId = candidateList.Id, SelectedCandidates = topCandidates.Count });
        }

        #endregion

        #region Get Pending CandidateLists

        [Authorize(Roles = "Manager")]
        [HttpGet("{postId}/PendingCandidateList")]
        public async Task<IActionResult> GetPendingCandidateList(int postId)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var manager = await _userManager.Users
                .Include(u => u.ManagedCompany)
                .FirstOrDefaultAsync(u => u.Email == userEmail);

            if (manager == null)
                return Unauthorized(new ApiResponse(401, "Manager not found"));

            var candidateLists = await _candidateListRepo
                .GetAllWithSpecAsync(new CandidateListWithApplicantsSpec(postId));

            if (candidateLists == null || !candidateLists.Any())
                return NotFound(new ApiResponse(404, "No pending candidate list found for this post."));

            var result = candidateLists.Select(cl =>
            {
                var applications = cl.CandidateListApplicants
                    .Select(cla => cla.Applicant)
                    .SelectMany(applicant => applicant.Applications
                        .Where(app => app.PostId == postId))
                    .OrderByDescending(app => app.RankScore)
                    .ToList();

                var applicantDtos = applications
                    .Select((app, index) => new PendingCandidateListApplicantDto
                    {
                        FullName = $"{app.Applicant.FName} {app.Applicant.LName}",
                        CV_Link = app.CV_Link,
                        Rank = index + 1
                    }).ToList();

                return new PendingCandidateListDto
                {
                    CandidateListId = cl.Id,
                    Applicants = applicantDtos
                };
            }).ToList();

            return Ok(result);
        }

        #endregion

        #region Candidate List Approval by manager

        [Authorize(Roles = "Manager")]
        [HttpPatch("CandidateList/{candidateListId}/Approval")]
        public async Task<IActionResult> ApproveCandidateList(int candidateListId, [FromBody] CandidateListApprovalDto request)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized(new ApiResponse(401, "User email not found in token"));

            var manager = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (manager == null)
                return Unauthorized(new ApiResponse(401, "Manager not found"));

            var candidateList = await _candidateListRepo.GetByIdAsync(candidateListId);
            if (candidateList == null || candidateList.ManagerId != manager.Id)
                return Forbid();

            if (candidateList.Status != "Pending")
                return BadRequest(new ApiResponse(400, "This list has already been processed."));

            candidateList.Status = request.IsApproved ? "Accepted" : "Rejected";

            await _candidateListRepo.UpdateAsync(candidateList);
            await _candidateListRepo.SaveChangesAsync();

            if (request.IsApproved)
            {
                return Ok(new { message = "Candidate list approved and moved to interview scheduling." });
            }
            return Ok(new { message = "Candidate list rejected." });
        }

        #endregion

        #region Display Hired Applicants

        [Authorize(Roles = "Agency")]
        [HttpGet("hired-applicants")]
        public async Task<IActionResult> GetHiredApplicants()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized(new ApiResponse(401, "User email not found in token"));

            var agency = await _userManager.FindByEmailAsync(userEmail);
            if (agency == null)
                return Unauthorized(new ApiResponse(401, "Agency not found"));

            var agencyApplicantsSpec = new AgencyApplicantsSpec(agency.Id);
            var agencyApplicants = await _agencyApplicantRepo.GetAllWithSpecAsync(agencyApplicantsSpec);

            var applicantIds = agencyApplicants.Select(aa => aa.ApplicantId).ToList();
            if (!applicantIds.Any())
                return Ok(new { HiredApplicants = new List<HiredApplicantDto>() });

            var hiredInterviewsSpec = new AgencyHiredApplicantsSpec(applicantIds);
            var hiredInterviews = await _interviewRepo.GetAllWithSpecAsync(hiredInterviewsSpec);

            if (!hiredInterviews.Any())
                return Ok(new { HiredApplicants = new List<HiredApplicantDto>() });

            var hiredApplicants = _mapper.Map<List<HiredApplicantDto>>(hiredInterviews);

            return Ok(new { HiredApplicants = hiredApplicants });
        }

        #endregion

        #region Apply Application

        [Authorize(Roles = "Agency")]
        [HttpPost("Posts/{postId}/SubmitApplication")]
        public async Task<IActionResult> SubmitApplication(int postId, [FromForm] SubmitApplicationDto dto)
        {
            var agencyEmail = User.FindFirstValue(ClaimTypes.Email);
            var agency = await _userManager.FindByEmailAsync(agencyEmail);
            if (agency == null)
                return Unauthorized(new ApiResponse(401, "Agency not found"));

            var spec = new PostByIdSpec(postId);
            var post = await _postRepo.GetByEntityWithSpecAsync(spec);
            if (post == null)
                return NotFound(new ApiResponse(404, "Post not found or not paid"));

            var applicant = _mapper.Map<Applicant>(dto);
            await _applicantRepo.AddAsync(applicant);
            await _applicantRepo.SaveChangesAsync();

            var agencyApplicant = new AgencyApplicant
            {
                AgencyId = agency.Id,
                ApplicantId = applicant.Id
            };
            await _agencyApplicantRepo.AddAsync(agencyApplicant);
            await _agencyApplicantRepo.SaveChangesAsync();

            var fileName = DocumentSettings.UploadFile(dto.CVFile, "CVs");
            var cvLink = $"/Files/CVs/{fileName}";

            var application = new Application
            {
                ApplicantId = applicant.Id,
                PostId = postId,
                AgencyId = agency.Id,
                ApplicationDate = DateTime.UtcNow,
                CV_Link = cvLink,
                RankScore = 0,
                IsShortlisted = false
            };
            await _applicationRepo.AddAsync(application);
            await _applicationRepo.SaveChangesAsync();

            return Ok(new ApiResponse(200, "Application submitted successfully"));
        }

        #endregion

    }
}