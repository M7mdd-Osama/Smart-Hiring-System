using Microsoft.AspNetCore.Mvc;
using SmartHiring.APIs.Errors;

namespace SmartHiring.APIs.Controllers
{
	[Route("errors/{code}")]
	[ApiController]
	[ApiExplorerSettings(IgnoreApi = true)]
	public class ErrorsController : ControllerBase
	{
		public ActionResult Error (int code)
		{
			return NotFound(new ApiResponse(code));
		}
	}
}
