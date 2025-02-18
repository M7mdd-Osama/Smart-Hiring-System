using SmartHiring.APIs.Errors;
using System.Net;
using System.Text.Json;

namespace SmartHiring.APIs.Middlewares
{
	public class ExceptionMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<ExceptionMiddleware> _logger;
		private readonly IHostEnvironment _env;

		public ExceptionMiddleware(RequestDelegate Next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
		{
			_next = Next;
			_logger = logger;
			_env = env;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				await _next.Invoke(context);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, ex.Message);
				context.Response.ContentType = "application/json";
				context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				//if(_env.IsDevelopment())
				//{
				//	var Response = new ApiExceptionResponse((int)HttpStatusCode.InternalServerError, ex.Message, ex.StackTrace.ToString());
				//}
				//else
				//{
				//	var Response = new ApiExceptionResponse((int)HttpStatusCode.InternalServerError);
				//}
				var Response = _env.IsDevelopment() ? new ApiExceptionResponse((int)HttpStatusCode.InternalServerError, ex.Message, ex.StackTrace.ToString())
					: new ApiExceptionResponse((int)HttpStatusCode.InternalServerError);
				var Options = new JsonSerializerOptions()
				{
					PropertyNamingPolicy = JsonNamingPolicy.CamelCase
				};
				var JsonResponse = JsonSerializer.Serialize(Response, Options);
				context.Response.WriteAsync(JsonResponse);
			}
		}
	}
}
