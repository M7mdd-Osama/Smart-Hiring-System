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
using System.Collections.Generic;
using System.Threading.Tasks;

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

        // ✅ [GET] Get All Posts
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PostToReturnDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PostToReturnDto>>> GetPosts()
        {
            var spec = new PostWithCompanySpecifications();
            var posts = await _postRepo.GetAllWithSpecAsync(spec);
            return Ok(_mapper.Map<IEnumerable<PostToReturnDto>>(posts));
        }

        // ✅ [GET] Get Post by ID
        [HttpGet("{jobId}")]
        [ProducesResponseType(typeof(PostToReturnDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PostToReturnDto>> GetPostById(int jobId)
        {
            var spec = new PostWithCompanySpecifications(jobId);
            var post = await _postRepo.GetByIdWithSpecAsync(spec);
            if (post == null) return NotFound(new ApiResponse(404));

            return Ok(_mapper.Map<PostToReturnDto>(post));
        }

        // ✅ [POST] Create Job Post
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

            if (hr.HRCompany == null)
                return BadRequest(new { message = "HR is not associated with a company" });

            var job = _mapper.Map<Post>(postDto);
            job.PostDate = DateTime.UtcNow;

            await _postRepo.AddAsync(job);

            return Ok(_mapper.Map<PostToReturnDto>(job));
        }

        // ✅ [PUT] Update Job Post
        [HttpPut("{jobId}")]
        public async Task<IActionResult> UpdateJob(int jobId, [FromBody] PostToReturnDto postDto)
        {
            if (postDto == null)
                return BadRequest(new { message = "Job data is required" });

            var job = await _postRepo.GetByIdAsync(jobId);
            if (job == null)
                return NotFound(new { message = "Job not found" });

            _mapper.Map(postDto, job); // ✅ تحديث الكائن مباشرة باستخدام AutoMapper
            await _postRepo.UpdateAsync(job);

            return Ok(new { message = "Job updated successfully", job = _mapper.Map<PostToReturnDto>(job) });
        }

        // ✅ [DELETE] Delete Job Post
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
