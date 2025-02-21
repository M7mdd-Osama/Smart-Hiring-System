using Microsoft.AspNetCore.Mvc;
using SmartHiring.APIs.Errors;
using SmartHiring.APIs.Helpers;
using SmartHiring.Core.Repositories;
using SmartHiring.Repository;

namespace SmartHiring.APIs.Extensions
{
	public static class ApplicationServicesExtension
	{
		public static IServiceCollection AddApplicationServices(this IServiceCollection Services)
		{
			Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

			Services.AddAutoMapper(typeof(MappingProfiles));

			#region Error Handling

			Services.Configure<ApiBehaviorOptions>(Options =>
			{
				Options.InvalidModelStateResponseFactory = (actionContext) =>
				{
					var errors = actionContext.ModelState.Where(P => P.Value.Errors.Count() > 0)
											  .SelectMany(P => P.Value.Errors)
											  .Select(E => E.ErrorMessage)
											  .ToArray();
					var ValidationErrorResponse = new ApiValidationErrorResponse()
					{
						Errors = errors
					};
					return new BadRequestObjectResult(ValidationErrorResponse);
				};
			});

			#endregion

			return Services;
		}
	}
}
