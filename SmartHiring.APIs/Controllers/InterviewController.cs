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
    public class InterviewController : APIBaseController
    {
        private readonly IGenericRepository<CandidateList> _candidateListRepo;
        private readonly IGenericRepository<CandidateListApplicant> _candidateListApplicantRepo;
        private readonly IGenericRepository<Interview> _interviewRepo;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly ImailSettings _mailSettings;

        public InterviewController(
            IGenericRepository<CandidateList> candidateListRepo,
            IGenericRepository<CandidateListApplicant> candidateListApplicantRepo,
            IGenericRepository<Interview> interviewRepo,
            UserManager<AppUser> userManager,
            IMapper mapper,
            ImailSettings mailSettings)
        {
            _candidateListRepo = candidateListRepo;
            _candidateListApplicantRepo = candidateListApplicantRepo;
            _interviewRepo = interviewRepo;
            _userManager = userManager;
            _mapper = mapper;
            _mailSettings = mailSettings;
        }

        #region Get Accepted CandidateLists

        [Authorize(Roles = "HR")]
        [HttpGet("accepted-candidate-lists")]
        public async Task<IActionResult> GetAcceptedCandidateLists()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized(new ApiResponse(401, "User email not found in token"));

            var user = await _userManager.Users
                .Include(u => u.HRCompany)
                .FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
                return Unauthorized(new ApiResponse(401, "User not found"));

            var spec = new AcceptedCandidateListsSpecification(user.HRCompany.Id);
            var candidateLists = await _candidateListRepo.GetAllWithSpecAsync(spec);

            if (candidateLists == null || !candidateLists.Any())
                return NotFound(new ApiResponse(404, "No accepted candidate lists found."));

            var result = _mapper.Map<List<CandidateListDto>>(candidateLists);

            return Ok(result);
        }

        #endregion

        #region Get Applicants In CandidateList

        [Authorize(Roles = "HR")]
        [HttpGet("{candidateListId}/applicants")]
        public async Task<IActionResult> GetApplicantsInCandidateList(int candidateListId)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized(new ApiResponse(401, "User email not found in token"));

            var hrUser = await _userManager.Users
                .Include(u => u.HRCompany)
                .FirstOrDefaultAsync(u => u.Email == userEmail);

            if (hrUser == null || hrUser.HRCompany == null)
                return Unauthorized(new ApiResponse(401, "HR user not found or does not belong to a company."));

            var spec = new CandidateListApplicantsSpecification(candidateListId, hrUser.Id);
            var candidateListApplicants = await _candidateListApplicantRepo.GetAllWithSpecAsync(spec);

            if (candidateListApplicants == null || !candidateListApplicants.Any())
                return NotFound(new ApiResponse(404, "No applicants found for this Candidate List."));

            var applicantsDto = _mapper.Map<IEnumerable<CandidateListApplicantDto>>(candidateListApplicants);

            return Ok(applicantsDto);
        }

        #endregion

        #region Schedule Interview

        [Authorize(Roles = "HR")]
        [HttpPost("schedule-interview")]
        public async Task<IActionResult> ScheduleInterview([FromQuery] int candidateListId, [FromQuery] int applicantId, [FromBody] InterviewSchedulingDto dto)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized(new ApiResponse(401, "User email not found in token"));

            var hr = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (hr == null)
                return Unauthorized(new ApiResponse(401, "HR not found"));

            var spec = new ApplicantSpecification(candidateListId, applicantId);
            var candidateListApplicant = await _candidateListApplicantRepo.GetByEntityWithSpecAsync(spec);

            if (candidateListApplicant == null)
                return NotFound(new ApiResponse(404, "Applicant not found in Candidate List."));

            var applicant = candidateListApplicant.Applicant;
            var post = candidateListApplicant.CandidateList.Post;
            var company = post.Company;

            if (applicant == null || post == null || company == null)
                return NotFound(new ApiResponse(404, "Applicant, Job Post, or Company not found."));

            var interview = _mapper.Map<Interview>(dto);
            interview.HRId = hr.Id;
            interview.PostId = post.Id;
            interview.ApplicantId = applicant.Id;

            await _interviewRepo.AddAsync(interview);
            await _interviewRepo.SaveChangesAsync();

            var agency = applicant.Applications.FirstOrDefault()?.Agency;
			var agencyName = agency != null ? agency.AgencyName : "N/A";

            var emailBody = $@"
					<h3>Dear {applicant.FName} {applicant.LName},</h3>
					<p>We are pleased to inform you that you have been shortlisted for an interview for the <b>{post.JobTitle}</b> position at <b>{company.Name}</b>.</p>

					<p>Your application was evaluated through our <b>AI-powered Smart Hiring System</b>, which identified you as a highly suitable candidate for this role based on your skills, experience, and qualifications.</p>

					<p>Your application was submitted via <b>{agencyName}</b>, and we appreciate your interest in this opportunity.</p>

					<p><b>Interview Details:</b></p>
					<ul>
						<li><b>Date:</b> {dto.Date:yyyy-MM-dd}</li>
						<li><b>Time:</b> {dto.Time}</li>
						<li><b>Location:</b> {dto.Location}</li>
					</ul>

					<p>If you need to reschedule or have any questions, please contact us at <b>{company.BusinessEmail}</b>.</p>

					<p>We look forward to meeting you and wish you the best of luck in your interview.</p>

					<p>Best regards,</p>
					<p><b>Smart Hiring Team</b></p>";

            var email = new Email
            {
                To = applicant.Email,
                Subject = "Interview Invitation - Smart Hiring",
                Body = emailBody
            };

            await _mailSettings.SendMail(email, false);

            var response = new
            {
                message = "Interview scheduled successfully and email sent.",
                interviewId = interview.Id
            };

            return Ok(response);
        }

        #endregion

        #region Update Interview Status

        [Authorize(Roles = "HR")]
        [HttpPatch("update-interview-status")]
        public async Task<IActionResult> UpdateInterviewStatus([FromQuery] int interviewId, [FromBody] UpdateInterviewStatusDto dto)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized(new ApiResponse(401, "User email not found in token"));

            var hr = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (hr == null)
                return Unauthorized(new ApiResponse(401, "HR not found"));

            var spec = new InterviewSpecification(interviewId);
            var interview = await _interviewRepo.GetByEntityWithSpecAsync(spec);
            if (interview == null)
                return NotFound(new ApiResponse(404, "Interview not found"));

            var interviewDateTime = interview.Date.Add(interview.Time);
            if (interviewDateTime > DateTime.UtcNow)
                return BadRequest(new ApiResponse(400, "Interview has not yet taken place"));

            if (dto.Status != "Hired" && dto.Status != "Rejected")
                return BadRequest(new ApiResponse(400, "Invalid status. Use 'Hired' or 'Rejected'"));

            interview.InterviewStatus = dto.Status == "Hired" ? InterviewStatus.Hired : InterviewStatus.Rejected;
            interview.HRId = hr.Id;

            await _interviewRepo.SaveChangesAsync();

            if (dto.Status == "Hired" && interview.Applicant.Applications.Any())
            {
                var application = interview.Applicant.Applications.FirstOrDefault(a => a.PostId == interview.PostId);
                var agency = application?.Agency;

                if (agency != null)
                {
                    var emailBody = $@"
					<h3>Dear {agency.FirstName} {agency.LastName},</h3>

					<p>We are pleased to inform you that a candidate submitted through your agency, <b>{agency.AgencyName}</b>, has successfully secured a position!</p>

					<h4>- Candidate Details:</h4>
					<ul>
						<li><b>Name:</b> {interview.Applicant.FName} {interview.Applicant.LName}</li>
						<li><b>Email:</b> {interview.Applicant.Email}</li>
						<li><b>Phone:</b> {interview.Applicant.Phone}</li>
					</ul>

					<h4>- Job Details:</h4>
					<ul>
						<li><b>Position:</b> {interview.Post.JobTitle}</li>
						<li><b>Company:</b> {interview.Post.Company.Name}</li>
						<li><b>Business Contact:</b> {interview.Post.Company.BusinessEmail}</li>
					</ul>

					<p>We appreciate your efforts in connecting top talent with leading companies. To discuss commission details or any follow-up, please feel free to contact the business email above.</p>

					<p>Thank you for your continued partnership!</p>

					<p>Best regards,</p>
					<p><b>Smart Hiring Team</b></p>";

                    var email = new Email
                    {
                        To = agency.Email,
                        Subject = "Candidate Hired Notification - Smart Hiring",
                        Body = emailBody
                    };

                    await _mailSettings.SendMail(email, false);
                }
            }

            return Ok(new ApiResponse(200, $"Interview status updated to '{dto.Status}' successfully."));
        }

        #endregion

        #region Delete CandidateList

        [Authorize(Roles = "HR")]
        [HttpDelete("delete-candidate-list/{candidateListId}")]
        public async Task<IActionResult> DeleteCandidateList(int candidateListId)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized(new ApiResponse(401, "User email not found in token"));

            var user = await _userManager.Users
                .Include(u => u.HRCompany)
                .FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
                return Unauthorized(new ApiResponse(401, "User not found"));

            var candidateList = await _candidateListRepo.GetByEntityWithSpecAsync(
                new CandidateListWithManagerSpecifications(candidateListId)
            );

            if (candidateList == null)
                return NotFound(new ApiResponse(404, "Candidate list not found."));

            if (candidateList.Manager == null || candidateList.Manager.ManagedCompany == null ||
                candidateList.Manager.ManagedCompany.Id != user.HRCompany.Id)
            {
                return Forbid();
            }

            await _candidateListRepo.DeleteAsync(candidateList);
            await _candidateListRepo.SaveChangesAsync();

            return Ok(new ApiResponse(200, "Candidate list deleted successfully."));
        }

        #endregion

    }
}