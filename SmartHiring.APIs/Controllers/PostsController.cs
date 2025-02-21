using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartHiring.APIs.DTOs;
using SmartHiring.APIs.Errors;
using SmartHiring.Core.Entities;
using SmartHiring.Core.Repositories;
using SmartHiring.Core.Specifications;

namespace SmartHiring.APIs.Controllers
{
	public class PostsController : APIsBaseController
	{
		private readonly IGenericRepository<Post> _postRepo;
		private readonly IMapper _mapper;
		public PostsController(IGenericRepository<Post> PostRepo, IMapper mapper)
		{
			_postRepo = PostRepo;
			_mapper = mapper;
		}
		[HttpGet]
		[ProducesResponseType(typeof(PostToReturnDto), StatusCodes.Status200OK)]
		public async Task<ActionResult<IReadOnlyList<Post>>> GetPosts()
		{
			var Spec = new PostWithCompanySpecifications();
			var Posts = await _postRepo.GetAllWithSpecAsync(Spec);
			var MappedPosts = _mapper.Map<IReadOnlyList<Post>, IReadOnlyList<PostToReturnDto>>(Posts);
			return Ok(MappedPosts);
		}
		[HttpGet("{id}")]
		[ProducesResponseType(typeof(PostToReturnDto), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
		public async Task<ActionResult<Post>> GetPostById(int id)
		{
			var Spec = new PostWithCompanySpecifications(id);
			var Post = await _postRepo.GetByIdWithSpecAsync(Spec);
			if (Post is null) return NotFound(new ApiResponse(404));
			var MappedPost = _mapper.Map<Post, PostToReturnDto>(Post);
			return Ok(MappedPost);
		}
	}
}