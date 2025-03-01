using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartHiring.APIs.DTOs;
using SmartHiring.Core.Entities;
using SmartHiring.Core.Entities.Identity;
using SmartHiring.Core.Repositories;
using SmartHiring.Core.Specifications;
using System.ComponentModel.Design;

namespace SmartHiring.APIs.Controllers
{
	public class ReportsController : APIBaseController
	{
		//private readonly IGenericRepository<Interview> _interviewRepo;
		//private readonly UserManager<AppUser> _agencyRepo;
		//private readonly IMapper _mapper;

		//public ReportsController(
		//	IGenericRepository<Interview> interviewRepo,
		//	UserManager<AppUser> agencyRepo,
		//	IMapper mapper)
		//{
		//	_interviewRepo = interviewRepo;
		//	_agencyRepo = agencyRepo;
		//	_mapper = mapper;
		//}

		//[HttpGet()]
		//[ProducesResponseType(typeof(InterviewReportToReturnDto), StatusCodes.Status200OK)]
		//public async Task<ActionResult<InterviewReportToReturnDto>> GetInterviewStageReport(DateTime fromDate, DateTime toDate, int companyId)
		//{
		//	var spec = new InterviewWithCandidateSpecifications();
		//	var interviews = await _interviewRepo.GetAllWithSpecAsync(spec);

		//	var filteredInterviews = interviews
		//		.Where(i => i.Date >= fromDate && i.Date <= toDate)
		//		.Where(i => i.HR.CompanyId == companyId)
		//		.ToList();

		//	var acceptedCandidates = filteredInterviews.Count(i => i.InterviewStatus == InterviewStatus.Accepted);
		//	var rejectedCandidates = filteredInterviews.Count(i => i.InterviewStatus == InterviewStatus.Rejected);
		//	var totalCandidates = acceptedCandidates + rejectedCandidates;

		//	var mappedCandidates = _mapper.Map<List<CandidateReportToReturnDto>>(
		//		filteredInterviews.Where(i => i.InterviewStatus != InterviewStatus.Pending)
		//	);

		//	var report = new InterviewReportToReturnDto
		//	{
		//		TotalCandidates = totalCandidates,
		//		AcceptedCandidates = acceptedCandidates,
		//		RejectedCandidates = rejectedCandidates,
		//		Candidates = mappedCandidates
		//	};
		//	return Ok(report);
		//}
	}
}