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
using SmartHiring.Core.Repositories;
using SmartHiring.Core.Specifications;
using System.Security.Claims;

namespace SmartHiring.APIs.Controllers
{
	public class PostsController : APIBaseController
	{
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
		private readonly UserManager<AppUser> _userManager;
		private readonly IPostRepo _postRepository;

		public PostsController(
            IUnitOfWork unitOfWork,
             IMapper mapper,
			 UserManager<AppUser> userManager,
			 IPostRepo postRepository)
		{
            _unitOfWork = unitOfWork;
            _mapper = mapper;
			_userManager = userManager;
			_postRepository = postRepository;
		}

        #region Get All Posts for HR & Manager

        [Authorize(Roles = "HR,Manager")]
        [HttpGet("hr-manager-posts")]
        public async Task<ActionResult> GetHRManagerPosts([FromQuery] PostSpecParams Params)
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
            int? companyId = userRole == "HR" ? user.HRCompany?.Id : user.ManagedCompany?.Id;

            var spec = new PostWithCompanySpec(Params, companyId, userRole, user.Id, false);
            var jobs = await _unitOfWork.Repository<Post>().GetAllWithSpecAsync(spec);

            if (!jobs.Any())
                return NotFound(new ApiResponse(404, "No posts found"));

            var savedPostIds = (await _unitOfWork.Repository<SavedPost>().GetAllWithSpecAsync(new SavedPostSpec(user.Id)))
                                    .Select(s => s.PostId)
                                    .ToHashSet();

            var countSpec = new PostWithFiltrationForCountAsync(Params, companyId, userRole, userRole == "Manager");
            var count = await _unitOfWork.Repository<Post>().GetCountWithSpecAsync(countSpec);

            var mappedPosts = _mapper.Map<IReadOnlyList<PostToReturnDto>>(jobs);

            foreach (var post in jobs)
            {
                var unreadNotes = post.Notes?.Any(note => !note.IsSeen && note.UserId != user.Id) ?? false;
                var postDto = mappedPosts.FirstOrDefault(dto => dto.Id == post.Id);

                if (postDto != null)
                {
                    postDto.HasUnreadNotes = unreadNotes;

                    if (userRole == "Manager")
                    {
                        var hasPendingCandidateList = post.CandidateLists?.Any(cl => cl.Status == "Pending") ?? false;
                        postDto.HasPendingCandidateList = hasPendingCandidateList;
                    }
                    else
                    {
                        postDto.HasPendingCandidateList = false;
                    }
                }
            }

            foreach (var post in mappedPosts)
                post.IsSaved = savedPostIds.Contains(post.Id);

            return Ok(new Pagination<PostToReturnDto>(Params.PageIndex, Params.PageSize, mappedPosts, count));
        }

        #endregion

        #region Get All Posts for Agency

        [Authorize(Roles = "Agency")]
        [HttpGet("agency-posts")]
        public async Task<ActionResult> GetAgencyPosts([FromQuery] PostSpecParams Params)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized(new ApiResponse(401, "User email not found in token"));
            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null)
                return Unauthorized(new ApiResponse(401, "User not found"));

            var spec = new PostWithCompanySpec(Params, null, "Agency", user.Id, true);
            var jobs = await _unitOfWork.Repository<Post>().GetAllWithSpecAsync(spec);

            if (!jobs.Any())
                return NotFound(new ApiResponse(404, "No posts found"));

            var savedPostIds = (await _unitOfWork.Repository<SavedPost>()
                                    .GetAllWithSpecAsync(new SavedPostSpec(user.Id)))
                                    .Select(s => s.PostId)
                                    .ToHashSet();

            var countSpec = new PostWithFiltrationForCountAsync(Params, null, "Agency", true);
            var count = await _unitOfWork.Repository<Post>().GetCountWithSpecAsync(countSpec);

            var mappedAgencyPosts = _mapper.Map<IReadOnlyList<PostToReturnForAgencyDto>>(jobs);

            foreach (var post in mappedAgencyPosts)
                post.IsSaved = savedPostIds.Contains(post.Id);

            return Ok(new Pagination<PostToReturnForAgencyDto>(Params.PageIndex,
                Params.PageSize, mappedAgencyPosts, count));
        }

        #endregion

        #region Get Post By Id

        [Authorize(Roles = "HR,Manager,Agency")]
        [HttpGet("{postId}")]
        public async Task<ActionResult<PostToReturnDto>> GetPostById(int postId)
        {
            var spec = new PostWithCompanySpec(postId);
            var post = await _unitOfWork.Repository<Post>().GetByEntityWithSpecAsync(spec);

            if (post == null || post.PaymentStatus != "Paid")
                return NotFound(new ApiResponse(404, "Post not found"));

            return Ok(_mapper.Map<PostToReturnDto>(post));
        }

        #endregion

        #region Save & Unsave Post (HR, Manager, Agency)

        [Authorize(Roles = "HR,Manager,Agency")]
        [HttpPatch("post_save_status")]
        public async Task<IActionResult> UpdateSaveStatus([FromQuery] int post_id,
                                                          [FromBody] SaveStatusDto request)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized(new ApiResponse(401, "User email not found in token"));

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null)
                return Unauthorized(new ApiResponse(401, "User not found"));

            var post = await _unitOfWork.Repository<Post>().GetByIdAsync(post_id);
            if (post == null)
                return NotFound(new ApiResponse(404, "Job not found"));

            var savedPost = await _unitOfWork.Repository<SavedPost>()
                                             .GetFirstOrDefaultAsync(s => s.UserId == user.Id &&
                                             s.PostId == post_id);
            if (request.Status)
            {
                if (savedPost == null)
                {
                    var newSavedPost = new SavedPost { UserId = user.Id, PostId = post_id };
                    await _unitOfWork.Repository<SavedPost>().AddAsync(newSavedPost);
                }
            }
            else
            {
                if (savedPost != null)
                {
                    await _unitOfWork.Repository<SavedPost>().DeleteAsync(savedPost);
                }
            }

            return Ok(new { message = "Save status updated successfully", save_status = request.Status });
        }

        #endregion

        #region Get Saved Posts for HR & Manager

        [Authorize(Roles = "HR,Manager")]
        [HttpGet("hr-manager-saved")]
        public async Task<ActionResult<IReadOnlyList<PostToReturnDto>>> GetSavedPostsForHRManager()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized(new ApiResponse(401, "User email not found in token"));

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null)
                return Unauthorized(new ApiResponse(401, "User not found"));

            var savedPostSpec = new SavedPostSpec(user.Id);
            var savedPosts = await _unitOfWork.Repository<SavedPost>().GetAllWithSpecAsync(savedPostSpec);

            if (!savedPosts.Any())
                return NotFound(new ApiResponse(404, "No saved posts found"));

            var postIds = savedPosts.Select(s => s.PostId).ToList();

            var spec = new PostWithCompanySpec(postIds);
            var posts = await _unitOfWork.Repository<Post>().GetAllWithSpecAsync(spec);

            var mappedPosts = _mapper.Map<IReadOnlyList<PostToReturnDto>>(posts);

            foreach (var post in mappedPosts)
                post.IsSaved = true;

            return Ok(mappedPosts);
        }

        #endregion

        #region Get Saved Posts for Agency

        [Authorize(Roles = "Agency")]
		[HttpGet("agency-saved")]
		public async Task<ActionResult<IReadOnlyList<PostToReturnForAgencyDto>>> GetSavedPostsForAgency()
		{
			var userEmail = User.FindFirstValue(ClaimTypes.Email);
			if (string.IsNullOrEmpty(userEmail))
				return Unauthorized(new ApiResponse(401, "User email not found in token"));

			var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
			if (user == null)
				return Unauthorized(new ApiResponse(401, "User not found"));

			var savedPostSpec = new SavedPostSpec(user.Id);
			var savedPosts = await _unitOfWork.Repository<SavedPost>().GetAllWithSpecAsync(savedPostSpec);

			if (!savedPosts.Any())
				return NotFound(new ApiResponse(404, "No saved posts found"));

			var postIds = savedPosts.Select(s => s.PostId).ToList();

			var spec = new PostWithCompanySpec(postIds);
			var posts = await _unitOfWork.Repository<Post>().GetAllWithSpecAsync(spec);

			var mappedAgencyPosts = _mapper.Map<IReadOnlyList<PostToReturnForAgencyDto>>(posts);

			foreach (var post in mappedAgencyPosts)
				post.IsSaved = true;

			return Ok(mappedAgencyPosts);
		}

        #endregion

        #region Create & Update & Delete Post

        #region Create Post
        [Authorize(Roles = "HR")]
        [HttpPost]
        public async Task<IActionResult> CreatePost([FromBody] PostCreationDto postDto)
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

            await _unitOfWork.Repository<Post>().AddAsync(post);
            await _unitOfWork.CompleteAsync();

            return Ok(new { message = "Job created successfully", postId = post.Id });
        }

        #endregion

        #region Update Post
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

            post.JobTitle = updateDto.JobTitle ?? post.JobTitle;
            post.Country = updateDto.Country ?? post.Country;
            post.City = updateDto.City ?? post.City;
            post.Description = updateDto.Description ?? post.Description;
            post.Requirements = updateDto.Requirements ?? post.Requirements;
            post.Deadline = updateDto.Deadline ?? post.Deadline;
            post.MinSalary = updateDto.MinSalary ?? post.MinSalary;
            post.MaxSalary = updateDto.MaxSalary ?? post.MaxSalary;
            post.Currency = updateDto.Currency ?? post.Currency;
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

            await _postRepository.UpdateAsync(post);
            await _postRepository.SaveChangesAsync();

            return Ok(new ApiResponse(200, "Post updated successfully"));
        }

        #endregion

        #region Delete Post
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

		#endregion

	}
}