using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartHiring.APIs.DTOs;
using SmartHiring.Core.Entities;
using SmartHiring.Core.Entities.Identity;
using SmartHiring.Core.Repositories;
using SmartHiring.Core.Specifications;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartHiring.APIs.Controllers
{
    public class InterviewController : APIBaseController
    {
        private readonly IGenericRepository<Interview> _interviewRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly IGenericRepository<Applicant> _applicantRepository;
        private readonly IGenericRepository<Post> _postRepository;
        private readonly IMapper _mapper;

        public InterviewController(
            IGenericRepository<Interview> interviewRepository,
            UserManager<AppUser> userManager,
            IGenericRepository<Applicant> applicantRepository,
            IGenericRepository<Post> postRepository,
            IMapper mapper)
        {
            _interviewRepository = interviewRepository;
            _userManager = userManager;
            _applicantRepository = applicantRepository;
            _postRepository = postRepository;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<InterviewDto>> ScheduleInterview([FromBody] InterviewDto interviewDto)
        {
            if (interviewDto == null)
                return BadRequest("Invalid interview data.");

            if (string.IsNullOrWhiteSpace(interviewDto.HRId) || interviewDto.ApplicantId <= 0 || interviewDto.PostId <= 0)
                return BadRequest("HRId, ApplicantId, and PostId must be valid.");

            var hr = await _userManager.FindByIdAsync(interviewDto.HRId);
            if (hr == null)
                return NotFound("HR not found.");

            if (!await _userManager.IsInRoleAsync(hr, "HR"))
                return BadRequest("User provided is not an HR.");

            var spec = new InterviewWithCandidateSpecifications(interviewDto.ApplicantId, interviewDto.HRId);
            var existingInterview = await _interviewRepository.GetByIdWithSpecAsync(spec);

            if (existingInterview != null
                && existingInterview.Date == interviewDto.Date
                && existingInterview.Time.ToString(@"hh\:mm") == interviewDto.Time)
            {
                return Conflict("This applicant already has an interview scheduled at the same time.");
            }

            var interview = _mapper.Map<Interview>(interviewDto);
            interview.InterviewStatus = InterviewStatus.Pending;
            interview.Score = 0;

            await _interviewRepository.AddAsync(interview);
            var result = _mapper.Map<InterviewDto>(interview);

            return Ok(result);
        }

        [HttpGet("{interviewId}")]
        public async Task<ActionResult<InterviewDto>> GetInterviewById(int interviewId)
        {
            var spec = new InterviewWithCandidateSpecifications(interviewId);
            var interview = await _interviewRepository.GetByIdWithSpecAsync(spec);
            if (interview == null)
                return NotFound($"Interview with ID {interviewId} not found.");

            return Ok(_mapper.Map<InterviewDto>(interview));
        }

        [HttpGet("job/{jobId}")]
        public async Task<ActionResult<IEnumerable<InterviewDto>>> GetInterviewsByJobId(int jobId)
        {
            var spec = new InterviewWithCandidateSpecifications();
            var interviews = await _interviewRepository.GetAllWithSpecAsync(spec);
            var jobInterviews = interviews.Where(i => i.PostId == jobId);

            if (!jobInterviews.Any())
                return NotFound($"No interviews found for job ID {jobId}.");

            return Ok(_mapper.Map<IEnumerable<InterviewDto>>(jobInterviews));
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

            return Ok(_mapper.Map<InterviewDto>(interview));
        }
    }
}
