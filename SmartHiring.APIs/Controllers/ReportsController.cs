using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

		public ReportsController(
			IGenericRepository<Interview> interviewRepo,
			UserManager<AppUser> agencyRepo,
			IMapper mapper)
		{
			_interviewRepo = interviewRepo;
			_agencyRepo = agencyRepo;
			_mapper = mapper;
		}

		[Authorize(Roles = "HR,Manager")]
		[HttpGet()]
		[ProducesResponseType(typeof(InterviewReportToReturnDto), StatusCodes.Status200OK)]
		public async Task<ActionResult<InterviewReportToReturnDto>> GetInterviewStageReport(DateTime fromDate, DateTime toDate)
		{
			var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

			if (string.IsNullOrEmpty(userEmail))
				return Unauthorized(new ApiResponse(401, "User email not found"));

			var user = await _agencyRepo.Users
				.Include(u => u.HRCompany)
				.Include(u => u.ManagedCompany)
				.FirstOrDefaultAsync(u => u.Email == userEmail);

			if (user == null)
				return Unauthorized(new ApiResponse(401, "User not found"));

			Console.WriteLine($"User: {user.Email}, HRCompany: {user.HRCompany?.Name}, ManagedCompany: {user.ManagedCompany?.Name}");

			Company? userCompany = null;

			if (User.IsInRole("HR"))
			{
				userCompany = user.HRCompany;
				Console.WriteLine("Role: HR");
			}
			else if (User.IsInRole("Manager"))
			{
				userCompany = user.ManagedCompany;
				Console.WriteLine("Role: Manager");
			}

			if (userCompany == null)
			{
				Console.WriteLine("User company not found!");
				return BadRequest(new ApiResponse(400, "User is not associated with any company"));
			}

			Console.WriteLine($"User company: {userCompany.Name}");

			var spec = new InterviewWithCandidateSpecifications();
			var interviews = await _interviewRepo.GetAllWithSpecAsync(spec);

			var filteredInterviews = interviews
				.Where(i => i.Date >= fromDate && i.Date <= toDate)
				.Where(i => i.HR != null && i.HR.HRCompany != null && i.HR.HRCompany.Id == userCompany.Id)
				.ToList();

			var acceptedCandidates = filteredInterviews.Count(i => i.InterviewStatus == InterviewStatus.Accepted);
			var rejectedCandidates = filteredInterviews.Count(i => i.InterviewStatus == InterviewStatus.Rejected);
			var totalCandidates = acceptedCandidates + rejectedCandidates;

			var mappedCandidates = _mapper.Map<List<CandidateReportToReturnDto>>(
				filteredInterviews.Where(i => i.InterviewStatus != InterviewStatus.Pending)
			);

			var report = new InterviewReportToReturnDto
			{
				TotalCandidates = totalCandidates,
				AcceptedCandidates = acceptedCandidates,
				RejectedCandidates = rejectedCandidates,
				Candidates = mappedCandidates
			};

			return Ok(report);
		}
	}
}