using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartHiring.APIs.Errors;
using SmartHiring.Repository.Data;

namespace SmartHiring.APIs.Controllers
{
	public class BuggyController : APIBaseController
	{
		private readonly SmartHiringDbContext _dbContext;

		public BuggyController(SmartHiringDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		[HttpGet("NotFound")]
		public ActionResult GetNotFoundRequest()
		{
			var Post = _dbContext.Posts.Find(100);

			if (Post is null) return NotFound(new ApiResponse(404));

			return Ok(Post);
		}

		[HttpGet("ServerError")]
		public ActionResult GetServerError()
		{
			var Post = _dbContext.Posts.Find(100);

			var PostToReturn = Post.ToString();

			return Ok(PostToReturn);
		}

		[HttpGet("BadRequest")]
		public ActionResult GetbadRequest()
		{
			return BadRequest();
		}

		[HttpGet("BadRequest/{id}")]
		public ActionResult GetBadRequest(int id)
		{
			return Ok();
		}
	}
}
