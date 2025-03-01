using AutoMapper;
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

namespace SmartHiring.APIs.Controllers
{
	public class PostsController : APIBaseController
	{
		private readonly IGenericRepository<Post> _postRepo;
		private readonly IMapper _mapper;
		private readonly UserManager<AppUser> _userManager;

		public PostsController(IGenericRepository<Post> postRepo, IMapper mapper, UserManager<AppUser> userManager)
		{
			_postRepo = postRepo;
			_mapper = mapper;
			_userManager = userManager;
		}

		[HttpGet]
		[ProducesResponseType(typeof(PostToReturnDto), StatusCodes.Status200OK)]
		public async Task<ActionResult<IEnumerable<Post>>> GetPosts()
		{
			var spec = new PostWithCompanySpecifications();
			var posts = await _postRepo.GetAllWithSpecAsync(spec);
			var mappedPosts = _mapper.Map<IEnumerable<Post>, IEnumerable<PostToReturnDto>>(posts);
			return Ok(mappedPosts);
		}

		[HttpGet("{jobId}")]
		[ProducesResponseType(typeof(PostToReturnDto), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
		public async Task<ActionResult<Post>> GetPostById(int jobId)
		{
			var spec = new PostWithCompanySpecifications(jobId);
			var post = await _postRepo.GetByIdWithSpecAsync(spec);
			if (post is null) return NotFound(new ApiResponse(404));
			var mappedPost = _mapper.Map<Post, PostToReturnDto>(post);
			return Ok(mappedPost);
		}

		[HttpPost]
		public async Task<ActionResult<PostToReturnDto>> CreateJob([FromBody] PostToReturnDto postDto)
		{
			if (postDto == null)
				return BadRequest(new { message = "Job data is required" });

			var hr = await _userManager.Users
				.Include(u => u.HRCompany) 
				.FirstOrDefaultAsync(u => u.Id == postDto.HRId);

			if (hr == null)
				return NotFound(new { message = "HR not found" });

			Console.WriteLine($"HR Id: {hr.Id}");
			Console.WriteLine($"HR Name: {hr.DisplayName}");
			Console.WriteLine($"HRCompany: {hr.HRCompany}");
			Console.WriteLine($"HRCompany Name: {hr.HRCompany?.Name}");

			if (hr.HRCompany == null)
				return BadRequest(new { message = "HR is not associated with a company" });

			var job = new Post
			{
				JobTitle = postDto.JobTitle,
				Description = postDto.Description,
				Requirements = postDto.Requirements,
				PostDate = DateTime.UtcNow,
				Deadline = postDto.Deadline,
				JobSalary = postDto.JobSalary,
				JobStatus = "Open",
				HRId = postDto.HRId
			};

			await _postRepo.AddAsync(job);

			var result = _mapper.Map<Post, PostToReturnDto>(job);
			return Ok(result);
		}

		[HttpPut("{jobId}")]
		public async Task<IActionResult> UpdateJob(int jobId, [FromBody] PostToReturnDto postDto)
		{
			if (postDto == null)
				return BadRequest(new { message = "Job data is required" });

			var job = await _postRepo.GetByIdAsync(jobId);
			if (job == null)
				return NotFound(new { message = "Job not found" });

			job.JobTitle = postDto.JobTitle;
			job.Description = postDto.Description;
			job.Requirements = postDto.Requirements;
			job.Deadline = postDto.Deadline;
			job.JobSalary = postDto.JobSalary;
			job.JobStatus = postDto.JobStatus;

			await _postRepo.UpdateAsync(job);

			return Ok(new
			{
				message = "Job updated successfully",
				job = new
				{
					job.Id,
					job.JobTitle,
					job.Description,
					job.Requirements,
					job.Deadline,
					job.JobSalary,
					job.JobStatus
				}
			});
		}

		[HttpDelete("{jobId}")]
		public async Task<IActionResult> DeleteJob(int jobId)
		{
			var job = await _postRepo.GetByIdAsync(jobId);
			if (job == null)
				return NotFound(new { message = "Job not found" });

			await _postRepo.DeleteAsync(job);

			return Ok(new { message = "Job deleted successfully" });
		}
	}
}