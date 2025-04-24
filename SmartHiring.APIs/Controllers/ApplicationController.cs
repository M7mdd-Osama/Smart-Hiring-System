using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartHiring.APIs.DTOs;
using SmartHiring.APIs.Errors;
using SmartHiring.APIs.Helpers;
using SmartHiring.Core;
using SmartHiring.Core.Entities;
using SmartHiring.Core.Entities.Identity;
using SmartHiring.Core.Services;
using SmartHiring.Core.Specifications;
using System.Security.Claims;

namespace SmartHiring.APIs.Controllers
{
    public class ApplicationController : APIBaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        private readonly PdfTextExtractor _pdfTextExtractor;
        private readonly IResumeEvaluationService _resumeEvaluationService;
        private readonly ILogger<ApplicationController> _logger;

        public ApplicationController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            UserManager<AppUser> userManager,
            PdfTextExtractor pdfTextExtractor,
            IResumeEvaluationService resumeEvaluationService,
            ILogger<ApplicationController> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
            _pdfTextExtractor = pdfTextExtractor;
            _resumeEvaluationService = resumeEvaluationService;
            _logger = logger;
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

            var post = await _unitOfWork.Repository<Post>().GetByIdAsync(postId);

            if (post == null || post.CompanyId != user.HRCompany.Id)
                return Forbid();

            var spec = new ApplicationsByPostIdSpec(postId);
            var allApplications = await _unitOfWork.Repository<Application>().GetAllWithSpecAsync(spec);

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

            var post = await _unitOfWork.Repository<Post>().GetByIdAsync(postId);

            if (post == null || post.CompanyId != user.HRCompany.Id)
                return Forbid();

            var spec = new AcceptedApplicationsByPostIdSpec(postId);
            var filteredCandidates = await _unitOfWork.Repository<Application>().GetAllWithSpecAsync(spec);

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
            var candidateLists = await _unitOfWork.Repository<CandidateList>().GetAllAsync();
            var isSent = candidateLists.Any(cl => cl.PostId == postId &&
                                            (cl.Status == "Pending" ||
                                            cl.Status == "Accepted"));
            var result = new
            {
                post.Deadline,
                TotalCount = mappedCandidates.Count,
                IsSent = isSent,
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

            var post = await _unitOfWork.Repository<Post>().GetByIdAsync(postId);
            if (post == null || post.CompanyId != user.HRCompany.Id)
                return Forbid();

            var candidateLists = await _unitOfWork.Repository<CandidateList>().GetAllAsync();
            var existingList = candidateLists.Any(cl => cl.PostId == postId && (cl.Status == "Pending" || cl.Status == "Accepted"));

            if (existingList)
                return BadRequest(new ApiResponse(400, "Cannot create a new candidate list. There's already one Pending or Accepted."));

            var manager = await _userManager.Users
                .FirstOrDefaultAsync(u => u.ManagedCompany != null && u.ManagedCompany.Id == user.HRCompany.Id);

            if (manager == null)
                return NotFound(new ApiResponse(404, "No manager found for this company."));

            var spec = new AcceptedApplicationsByPostIdSpec(postId);
            var filteredCandidates = await _unitOfWork.Repository<Application>().GetAllWithSpecAsync(spec);

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

            await _unitOfWork.Repository<CandidateList>().AddAsync(candidateList);
            await _unitOfWork.CompleteAsync();

            var candidateListApplicants = topCandidates.Select(c => new CandidateListApplicant
            {
                CandidateListId = candidateList.Id,
                ApplicantId = c.ApplicantId
            }).ToList();

            await _unitOfWork.Repository<CandidateListApplicant>().AddRangeAsync(candidateListApplicants);
            await _unitOfWork.CompleteAsync();

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

            var candidateLists = await _unitOfWork.Repository<CandidateList>()
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
                    .Select((app, index) =>
                    {
                        var dto = _mapper.Map<PendingCandidateListApplicantDto>(app);
                        dto.Rank = index + 1;
                        return dto;
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
            if (manager == null) return Unauthorized(new ApiResponse(401, "Manager not found"));

            var candidateList = await _unitOfWork.Repository<CandidateList>().GetByIdAsync(candidateListId);
            if (candidateList == null || candidateList.ManagerId != manager.Id) return Forbid();

            if (candidateList.Status != "Pending")
                return BadRequest(new ApiResponse(400, "This list has already been processed."));

            candidateList.Status = request.IsApproved ? "Accepted" : "Rejected";

            await _unitOfWork.Repository<CandidateList>().UpdateAsync(candidateList);
            await _unitOfWork.CompleteAsync();

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
            var agencyApplicants = await _unitOfWork.Repository<AgencyApplicant>().GetAllWithSpecAsync(agencyApplicantsSpec);

            var applicantIds = agencyApplicants.Select(aa => aa.ApplicantId).ToList();
            if (!applicantIds.Any())
                return Ok(new { HiredApplicants = new List<HiredApplicantDto>() });

            var hiredInterviewsSpec = new AgencyHiredApplicantsSpec(applicantIds);
            var hiredInterviews = await _unitOfWork.Repository<Interview>().GetAllWithSpecAsync(hiredInterviewsSpec);

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
            var post = await _unitOfWork.Repository<Post>().GetByEntityWithSpecAsync(spec);
            if (post == null)
                return NotFound(new ApiResponse(404, "Post not found or not paid"));

            var applicant = _mapper.Map<Applicant>(dto);
            await _unitOfWork.Repository<Applicant>().AddAsync(applicant);
            await _unitOfWork.CompleteAsync();

            var agencyApplicant = new AgencyApplicant
            {
                AgencyId = agency.Id,
                ApplicantId = applicant.Id
            };
            await _unitOfWork.Repository<AgencyApplicant>().AddAsync(agencyApplicant);
            await _unitOfWork.CompleteAsync();

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
                IsShortlisted = false,
                IsEvaluatedByAI = false
            };
            await _unitOfWork.Repository<Application>().AddAsync(application);
            await _unitOfWork.CompleteAsync();

            try
            {
                var cvPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", cvLink.TrimStart('/'));
                var extractedText = await _pdfTextExtractor.ExtractTextFromPdfAsync(cvPath);
                application.ExtractedResumeText = extractedText;

                await _unitOfWork.Repository<Application>().UpdateAsync(application);
                await _unitOfWork.CompleteAsync();

                var prediction = await _resumeEvaluationService.EvaluateResumeAsync(postId, extractedText);

                if (prediction != null)
                {
                    application.RankScore = prediction.score;
                    application.IsShortlisted = prediction.classification.Equals("Accepted", StringComparison.OrdinalIgnoreCase);
                    application.IsEvaluatedByAI = true;

                    await _unitOfWork.Repository<Application>().UpdateAsync(application);
                    await _unitOfWork.CompleteAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"AI Evaluation Failed for Application {application.Id}");
                application.IsEvaluatedByAI = false;
                await _unitOfWork.Repository<Application>().UpdateAsync(application);
                await _unitOfWork.CompleteAsync();
            }

            return Ok(new ApiResponse(200, "Application submitted successfully"));
        }
        #endregion

    }
}