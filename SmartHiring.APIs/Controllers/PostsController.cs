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
	public class PostsController : APIBaseController
	{
		private readonly IGenericRepository<Post> _postRepo;
		private readonly IMapper _mapper;
		private readonly UserManager<AppUser> _userManager;
		private readonly IGenericRepository<SavedPost> _savedPostRepo;
		private readonly IPostRepository _postRepository;

		public PostsController(IGenericRepository<Post> postRepo,
			 IMapper mapper,
			 UserManager<AppUser> userManager,
			 IGenericRepository<SavedPost> savedPostRepo,
			 IPostRepository postRepository)
		{
			_postRepo = postRepo;
			_mapper = mapper;
			_userManager = userManager;
			_savedPostRepo = savedPostRepo;
			_postRepository = postRepository;
		}

        #region Get All Posts & Get Post By Id

        [Authorize(Roles = "HR,Manager,Agency")]
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<PostToReturnDto>>> GetPosts([FromQuery] PostSpecParams Params)
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

            var userRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
            int? companyId = userRole == "HR" ? user.HRCompany?.Id
                                        : userRole == "Manager" ? user.ManagedCompany?.Id
                                        : null;

            bool onlyPaid = userRole == "Manager";

            var spec = new PostWithCompanySpecifications(Params, companyId, userRole, user.Id);
            var jobs = await _postRepo.GetAllWithSpecAsync(spec);

            if (!jobs.Any())
                return NotFound(new ApiResponse(404, "No posts found"));

            var mappedPosts = _mapper.Map<IReadOnlyList<PostToReturnDto>>(jobs);

            foreach (var post in jobs)
            {
                var unreadNotes = post.Notes?.Any(note => !note.IsSeen && note.UserId != user.Id) ?? false;

                var postDto = mappedPosts.FirstOrDefault(dto => dto.Id == post.Id);
                if (postDto != null)
                {
                    postDto.HasUnreadNotes = unreadNotes;
                }
            }

            var savedPostIds = (await _savedPostRepo.GetAllWithSpecAsync(new SavedPostSpecification(user.Id)))
                                .Select(s => s.PostId)
                                .ToHashSet();

            foreach (var post in mappedPosts)
                post.IsSaved = savedPostIds.Contains(post.Id);

            var countSpec = new PostWithFiltrationForCountAsync(Params, companyId, userRole, onlyPaid);
            var count = await _postRepo.GetCountWithSpecAsync(countSpec);

            return Ok(new Pagination<PostToReturnDto>(Params.PageIndex, Params.PageSize, mappedPosts, count));
        }

        [Authorize(Roles = "HR,Manager,Agency")]
		[HttpGet("{postId}")]
		public async Task<ActionResult<PostToReturnDto>> GetPostById(int postId)
		{
			var spec = new PostWithCompanySpecifications(postId);
			var post = await _postRepo.GetByEntityWithSpecAsync(spec);

			if (post == null || post.PaymentStatus != "Paid")
				return NotFound(new ApiResponse(404, "Post not found"));

			return Ok(_mapper.Map<PostToReturnDto>(post));
		}

		#endregion

		#region Save & Unsave Post

		[Authorize(Roles = "HR,Manager,Agency")]
		[HttpPatch("post_save_status")]
		public async Task<IActionResult> UpdateSaveStatus([FromQuery] int post_id, [FromBody] SaveStatusDto request)
		{
			var userEmail = User.FindFirstValue(ClaimTypes.Email);

			if (string.IsNullOrEmpty(userEmail))
				return Unauthorized(new ApiResponse(401, "User email not found in token"));

			var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
			if (user == null)
				return Unauthorized(new ApiResponse(401, "User not found"));

			var post = await _postRepo.GetByIdAsync(post_id);
			if (post == null)
				return NotFound(new ApiResponse(404, "Job not found"));

			var savedPost = await _savedPostRepo.GetFirstOrDefaultAsync(s => s.UserId == user.Id && s.PostId == post_id);

			if (request.Status)
			{
				if (savedPost == null)
				{
					var newSavedPost = new SavedPost { UserId = user.Id, PostId = post_id };
					await _savedPostRepo.AddAsync(newSavedPost);
				}
			}
			else
			{
				if (savedPost != null)
				{
					await _savedPostRepo.DeleteAsync(savedPost);
				}
			}

			return Ok(new { message = "Save status updated successfully", save_status = request.Status });
		}

		[Authorize(Roles = "HR,Manager,Agency")]
		[HttpGet("saved")]
		public async Task<ActionResult<IReadOnlyList<PostToReturnDto>>> GetSavedPosts()
		{
			var userEmail = User.FindFirstValue(ClaimTypes.Email);
			if (string.IsNullOrEmpty(userEmail))
				return Unauthorized(new ApiResponse(401, "User email not found in token"));

			var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
			if (user == null)
				return Unauthorized(new ApiResponse(401, "User not found"));

			var savedPostSpec = new SavedPostSpecification(user.Id);
			var savedPosts = await _savedPostRepo.GetAllWithSpecAsync(savedPostSpec);

			if (!savedPosts.Any())
				return NotFound(new ApiResponse(404, "No saved posts found"));

			var postIds = savedPosts.Select(s => s.PostId).ToList();

			var spec = new PostWithCompanySpecifications(postIds);
			var posts = await _postRepo.GetAllWithSpecAsync(spec);

			var mappedPosts = _mapper.Map<IReadOnlyList<PostToReturnDto>>(posts);

			foreach (var post in mappedPosts)
				post.IsSaved = true;

			return Ok(mappedPosts);
		}

		#endregion

		#region Create & Update & Delete Post

		[Authorize(Roles = "HR")]
		[HttpPost]
		public async Task<IActionResult> CreateJob([FromBody] PostCreationDto postDto)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var userEmail = User.FindFirstValue(ClaimTypes.Email);
			if (string.IsNullOrEmpty(userEmail))
				return Unauthorized("User email not found in token");

			var hr = await _userManager.Users
				.Where(u => u.Email == userEmail)
				.Include(u => u.HRCompany)
				.FirstOrDefaultAsync();

			if (hr == null || hr.HRCompany == null)
				return BadRequest("HR or company not found.");

			var post = _mapper.Map<Post>(postDto);
			post.HRId = hr.Id;
			post.CompanyId = hr.HRCompany.Id;
			post.PostDate = DateTime.UtcNow;
			post.PaymentStatus = "Pending Payment";

			await _postRepo.AddAsync(post);
			await _postRepo.SaveChangesAsync();

			return Ok(new { message = "Job created successfully", postId = post.Id });
		}

		[Authorize(Roles = "HR")]
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdatePost(int id, [FromBody] PostUpdateDto updateDto)
		{
			var userEmail = User.FindFirstValue(ClaimTypes.Email);
			if (string.IsNullOrEmpty(userEmail))
				return Unauthorized(new ApiResponse(401, "User email not found in token"));

			var user = await _userManager.Users
				.Include(u => u.HRCompany)
				.FirstOrDefaultAsync(u => u.Email == userEmail);

			if (user == null || user.HRCompany == null)
				return Unauthorized(new ApiResponse(401, "HR user not associated with any company"));

			var post = await _postRepository.GetPostWithRelations(id);
			if (post == null || post.CompanyId != user.HRCompany.Id)
				return NotFound(new ApiResponse(404, "Post not found or not authorized"));

			if (post.PaymentStatus == "Paid")
			{
				if (updateDto.JobTitle != null || updateDto.JobCategories != null ||
					updateDto.JobTypes != null || updateDto.Workplaces != null ||
					updateDto.Country != null || updateDto.City != null)
				{
					return BadRequest(new ApiResponse(400, "Cannot update restricted fields after payment"));
				}

				post.Description = updateDto.Description ?? post.Description;
				post.Requirements = updateDto.Requirements ?? post.Requirements;
				post.Deadline = updateDto.Deadline ?? post.Deadline;
				post.MinSalary = updateDto.MinSalary ?? post.MinSalary;
				post.MaxSalary = updateDto.MaxSalary ?? post.MaxSalary;
				post.Currency = updateDto.Currency ?? post.Currency;
				post.HideSalary = updateDto.HideSalary ?? post.HideSalary;
				post.MinExperience = updateDto.MinExperience ?? post.MinExperience;
				post.MaxExperience = updateDto.MaxExperience ?? post.MaxExperience;

				if (updateDto.Skills != null)
				{
					post.PostSkills.Clear();
					post.PostSkills = updateDto.Skills
						.Select(skillName => new PostSkill { Skill = new Skill { SkillName = skillName } })
						.ToList();
				}

				if (updateDto.CareerLevels != null)
				{
					post.PostCareerLevels.Clear();
					post.PostCareerLevels = updateDto.CareerLevels
						.Select(levelId => new PostCareerLevel { CareerLevelId = levelId })
						.ToList();
				}
			}
			else
			{
				post.JobTitle = updateDto.JobTitle ?? post.JobTitle;
				post.Country = updateDto.Country ?? post.Country;
				post.City = updateDto.City ?? post.City;
				post.Description = updateDto.Description ?? post.Description;
				post.Requirements = updateDto.Requirements ?? post.Requirements;
				post.Deadline = updateDto.Deadline ?? post.Deadline;
				post.MinSalary = updateDto.MinSalary ?? post.MinSalary;
				post.MaxSalary = updateDto.MaxSalary ?? post.MaxSalary;
				post.Currency = updateDto.Currency ?? post.Currency;
				post.HideSalary = updateDto.HideSalary ?? post.HideSalary;
				post.MinExperience = updateDto.MinExperience ?? post.MinExperience;
				post.MaxExperience = updateDto.MaxExperience ?? post.MaxExperience;

				if (updateDto.JobCategories != null)
				{
					post.PostJobCategories.Clear();
					post.PostJobCategories = updateDto.JobCategories
						.Select(categoryName => new PostJobCategory { JobCategory = new JobCategory { Name = categoryName } })
						.ToList();
				}

				if (updateDto.JobTypes != null)
				{
					post.PostJobTypes.Clear();
					post.PostJobTypes = updateDto.JobTypes
						.Select(typeId => new PostJobType { JobTypeId = typeId })
						.ToList();
				}

				if (updateDto.Workplaces != null)
				{
					post.PostWorkplaces.Clear();
					post.PostWorkplaces = updateDto.Workplaces
						.Select(workplaceId => new PostWorkplace { WorkplaceId = workplaceId })
						.ToList();
				}

				if (updateDto.Skills != null)
				{
					post.PostSkills.Clear();
					post.PostSkills = updateDto.Skills
						.Select(skillName => new PostSkill { Skill = new Skill { SkillName = skillName } })
						.ToList();
				}

				if (updateDto.CareerLevels != null)
				{
					post.PostCareerLevels.Clear();
					post.PostCareerLevels = updateDto.CareerLevels
						.Select(levelId => new PostCareerLevel { CareerLevelId = levelId })
						.ToList();
				}
			}

			await _postRepository.UpdateAsync(post);
			await _postRepository.SaveChangesAsync();

			return Ok(new ApiResponse(200, "Post updated successfully"));
		}

		[Authorize(Roles = "HR")]
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeletePost(int id)
		{
			var userEmail = User.FindFirstValue(ClaimTypes.Email);
			if (string.IsNullOrEmpty(userEmail))
				return Unauthorized(new ApiResponse(401, "User email not found in token"));

			var user = await _userManager.Users
				.Include(u => u.HRCompany)
				.FirstOrDefaultAsync(u => u.Email == userEmail);

			if (user == null || user.HRCompany == null)
				return Unauthorized(new ApiResponse(401, "HR user not associated with any company"));

			var post = await _postRepository.GetPostWithRelations(id);
			if (post == null || post.CompanyId != user.HRCompany.Id)
				return NotFound(new ApiResponse(404, "Post not found or not authorized"));

			_postRepository.DeleteRelatedEntities(post);

			await _postRepository.DeleteAsync(post);

			await _postRepository.SaveChangesAsync();

			return Ok(new ApiResponse(200, "Post and all related data deleted successfully"));
		}

		#endregion
	}
}