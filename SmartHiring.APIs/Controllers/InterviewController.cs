using AutoMapper;
using MailKit.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using SmartHiring.APIs.DTOs;
using SmartHiring.APIs.Errors;
using SmartHiring.APIs.Helpers;
using SmartHiring.Core;
using SmartHiring.Core.Entities;
using SmartHiring.Core.Entities.Identity;
using SmartHiring.Core.Specifications;
using System.Security.Claims;

namespace SmartHiring.APIs.Controllers
{
    public class InterviewController : APIBaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly ImailSettings _mailSettings;

        public InterviewController(
            IUnitOfWork unitOfWork,
            UserManager<AppUser> userManager,
            IMapper mapper,
            ImailSettings mailSettings)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
            _mailSettings = mailSettings;
        }

        #region Get Accepted CandidateLists With Applicants

        [Authorize(Roles = "HR,Manager")]
        [HttpGet("accepted-candidate-lists-with-applicants")]
        public async Task<IActionResult> GetAcceptedCandidateListsWithApplicants()
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

            var companyId = user.HRCompany?.Id ?? user.ManagedCompany?.Id;

            if (companyId == null)
                return Unauthorized(new ApiResponse(401, "User is not associated with any company"));

            var spec = new AcceptedCandidateListsSpec(companyId.Value);
            var candidateLists = await _unitOfWork.Repository<CandidateList>().GetAllWithSpecAsync(spec);

            if (candidateLists == null || !candidateLists.Any())
                return NotFound(new ApiResponse(404, "No accepted candidate lists found."));

            var result = new List<CandidateListWithApplicantsDto>();

            foreach (var candidateList in candidateLists)
            {
                var applicantsSpec = new CandidateListApplicantsSpec(candidateList.Id, user.Id);
                var applicants = await _unitOfWork.Repository<CandidateListApplicant>().GetAllWithSpecAsync(applicantsSpec);

                var candidateListDto = _mapper.Map<CandidateListWithApplicantsDto>(candidateList);
                candidateListDto.Candidates = _mapper.Map<List<CandidateListApplicantDto>>(applicants);

                result.Add(candidateListDto);
            }

            return Ok(result);
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

            var spec = new ApplicantSpec(candidateListId, applicantId);
            var candidateListApplicant = await _unitOfWork.Repository<CandidateListApplicant>().GetByEntityWithSpecAsync(spec);

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

            await _unitOfWork.Repository<Interview>().AddAsync(interview);
            await _unitOfWork.CompleteAsync();

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

            var spec = new InterviewSpec(interviewId);
            var interview = await _unitOfWork.Repository<Interview>().GetByEntityWithSpecAsync(spec);
            if (interview == null)
                return NotFound(new ApiResponse(404, "Interview not found"));

            var interviewDateTime = interview.Date.Add(interview.Time);
            if (interviewDateTime > DateTime.UtcNow)
                return BadRequest(new ApiResponse(400, "Interview has not yet taken place"));

            if (dto.Status != "Hired" && dto.Status != "Rejected")
                return BadRequest(new ApiResponse(400, "Invalid status. Use 'Hired' or 'Rejected'"));

            interview.InterviewStatus = dto.Status == "Hired" ? InterviewStatus.Hired : InterviewStatus.Rejected;
            interview.HRId = hr.Id;
            interview.Score = dto.Score;

            await _unitOfWork.Repository<Interview>().UpdateAsync(interview);
            await _unitOfWork.CompleteAsync();

            var applicant = interview.Applicant;
            var post = interview.Post;
            var company = post.Company;

            if (dto.Status == "Rejected")
            {
                var emailBody = $@"
                    <h3>Dear {applicant.FName} {applicant.LName},</h3>
                    <p>Thank you for taking the time to interview for the <b>{post.JobTitle}</b> position at <b>{company.Name}</b>.</p>
                    <p>After careful consideration, we regret to inform you that we have decided to move forward with another candidate at this time.</p>
                    <p>Your interview score was: <b>{dto.Score}/100</b>.</p>
                    <p>We encourage you to apply again in the future, and we appreciate your interest in our company.</p>
                    <p>Best regards,</p>
                    <p><b>{company.Name} HR Team</b></p>";

                var rejectionEmail = new Email
                {
                    To = applicant.Email,
                    Subject = "Interview Result - Smart Hiring",
                    Body = emailBody
                };

                await _mailSettings.SendMail(rejectionEmail, false);
            }
            else if (dto.Status == "Hired")
            {
                var application = applicant.Applications.FirstOrDefault(a => a.PostId == post.Id);
                var agency = application?.Agency;

                if (agency != null)
                {
                    var contractPdf = ContractPdfGenerator.Generate(interview);

                    var htmlBody = $@"
                        <h3>Dear {agency.FirstName} {agency.LastName},</h3>

                        <p>We are pleased to inform you that a candidate submitted through your agency, <b>{agency.AgencyName}</b>, has successfully secured a position!</p>

                        <h4>- Candidate Details:</h4>
                        <ul>
                            <li><b>Name:</b> {applicant.FName} {applicant.LName}</li>
                            <li><b>Email:</b> {applicant.Email}</li>
                            <li><b>Phone:</b> {applicant.Phone}</li>
                        </ul>

                        <h4>- Job Details:</h4>
                        <ul>
                            <li><b>Position:</b> {post.JobTitle}</li>
                            <li><b>Company:</b> {company.Name}</li>
                            <li><b>Business Contact:</b> {company.BusinessEmail}</li>
                        </ul>

                        <p>We appreciate your efforts in connecting top talent with leading companies. To discuss commission details or any follow-up, please feel free to contact the business email above.</p>

                        <p>Thank you for your continued partnership!</p>

                        <p>Best regards,</p>
                        <p><b>Smart Hiring Team</b></p>";
                    
                    await _mailSettings.SendMailWithAttachmentAndReplyTo(
                        agency.Email,
                        "Hired Candidate - Employment Contract",
                        htmlBody,
                        contractPdf,
                        "EmploymentContract.pdf",
                        company.BusinessEmail,
                        company.Name);
                }
                else
                {
                    Console.WriteLine("❌ AGENCY IS NULL OR NOT LOADED");
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

            var candidateList = await _unitOfWork.Repository<CandidateList>().GetByEntityWithSpecAsync(
                new CandidateListWithManagerSpec(candidateListId)
            );

            if (candidateList == null)
                return NotFound(new ApiResponse(404, "Candidate list not found."));

            if (candidateList.Manager == null || candidateList.Manager.ManagedCompany == null ||
                candidateList.Manager.ManagedCompany.Id != user.HRCompany.Id)
            {
                return Forbid();
            }

            await _unitOfWork.Repository<CandidateList>().DeleteAsync(candidateList);
            await _unitOfWork.CompleteAsync();

            return Ok(new ApiResponse(200, "Candidate list deleted successfully."));
        }

        #endregion
    }
}