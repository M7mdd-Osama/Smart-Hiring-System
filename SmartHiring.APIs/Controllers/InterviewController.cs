using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartHiring.APIs.DTOs;
using SmartHiring.Core.Entities;
using SmartHiring.Core.Entities.Identity;
using SmartHiring.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartHiring.APIs.Controllers
{
	public class InterviewController : APIBaseController
	{
		private readonly IGenericRepository<Interview> _interviewRepository;
		private readonly UserManager<AppUser> _hrRepository;
		private readonly IGenericRepository<Applicant> _applicantRepository;
		private readonly IGenericRepository<Post> _postRepository;

		public InterviewController(
			IGenericRepository<Interview> interviewRepository,
			UserManager<AppUser> hrRepository,
			IGenericRepository<Applicant> applicantRepository,
			IGenericRepository<Post> postRepository)
		{
			_interviewRepository = interviewRepository;
			_hrRepository = hrRepository;
			_applicantRepository = applicantRepository;
			_postRepository = postRepository;
		}

		[HttpPost]
		public async Task<ActionResult> ScheduleInterview([FromBody] InterviewDto interviewDto)
		{
			if (interviewDto == null)
				return BadRequest("Invalid interview data.");

			if (string.IsNullOrWhiteSpace(interviewDto.HRId) || interviewDto.ApplicantId <= 0 || interviewDto.PostId <= 0)
				return BadRequest("HRId, ApplicantId, and PostId must be valid.");

			if (string.IsNullOrWhiteSpace(interviewDto.Location))
				return BadRequest("Location is required.");

			var hr = await _hrRepository.FindByIdAsync(interviewDto.HRId.ToString());
			if (hr == null)
				return NotFound("HR not found.");

			var isHR = await _hrRepository.IsInRoleAsync(hr, "HR");
			if (!isHR)
				return BadRequest("User provided is not an HR.");

			var applicant = await _applicantRepository.GetByIdAsync(interviewDto.ApplicantId);
			var post = await _postRepository.GetByIdAsync(interviewDto.PostId);

			if (applicant == null || post == null)
				return NotFound("Applicant or Post not found.");

			var existingInterview = (await _interviewRepository.GetAllAsync())
				.FirstOrDefault(i => i.ApplicantId == interviewDto.ApplicantId && i.Date == interviewDto.Date && i.Time == interviewDto.Time);

			if (existingInterview != null)
				return Conflict("This applicant already has an interview scheduled at the same time.");

			var interview = new Interview
			{
				Date = interviewDto.Date,
				Time = interviewDto.Time,
				Location = interviewDto.Location,
				InterviewStatus = InterviewStatus.Pending,
				Score = 0,
				HRId = interviewDto.HRId,
				ApplicantId = interviewDto.ApplicantId,
				PostId = interviewDto.PostId
			};

			await _interviewRepository.AddAsync(interview);

			return Ok(new
			{
				message = "Interview scheduled successfully.",
				interviewId = interview.Id
			});
		}

		[HttpGet("{interviewId}")]
		public async Task<ActionResult<InterviewDto>> GetInterviewById(int interviewId)
		{
			var interview = await _interviewRepository.GetByIdAsync(interviewId);
			if (interview == null)
				return NotFound($"Interview with ID {interviewId} not found.");

			var interviewDto = new InterviewDto
			{
				Id = interview.Id,
				Date = interview.Date,
				Time = interview.Time,
				Location = interview.Location,
				InterviewStatus = interview.InterviewStatus.ToString(),
				Score = interview.Score,
				HRId = interview.HRId,
				ApplicantId = interview.ApplicantId,
				PostId = interview.PostId
			};

			return Ok(interviewDto);
		}

		[HttpGet("job/{jobId}")]
		public async Task<ActionResult<IEnumerable<InterviewDto>>> GetInterviewsByJobId(int jobId)
		{
			var interviews = await _interviewRepository.GetAllAsync();
			var jobInterviews = interviews
				.Where(i => i.PostId == jobId)
				.Select(i => new InterviewDto
				{
					Id = i.Id,
					Date = i.Date,
					Time = i.Time,
					Location = i.Location,
					InterviewStatus = i.InterviewStatus.ToString(),
					Score = i.Score,
					HRId = i.HRId,
					ApplicantId = i.ApplicantId,
					PostId = i.PostId
				})
				.ToList();

			if (!jobInterviews.Any())
				return NotFound($"No interviews found for job ID {jobId}.");

			return Ok(jobInterviews);
		}

		[HttpPut("{interviewId}/result")]
		public async Task<IActionResult> UpdateInterviewResult(int interviewId, [FromBody] int score)
		{
			if (score < 0 || score > 100)
				return BadRequest("Invalid score. It should be between 0 and 100.");

			var interview = await _interviewRepository.GetByIdAsync(interviewId);
			if (interview == null)
				return NotFound("Interview not found.");

			interview.Score = score;
			await _interviewRepository.UpdateAsync(interview);

			var updatedDto = new InterviewDto
			{
				Id = interview.Id,
				Date = interview.Date,
				Time = interview.Time,
				Location = interview.Location,
				InterviewStatus = interview.InterviewStatus.ToString(),
				Score = interview.Score,
				HRId = interview.HRId,
				ApplicantId = interview.ApplicantId,
				PostId = interview.PostId
			};

			return Ok(updatedDto);
		}
	}
}
