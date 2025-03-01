using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SmartHiring.Core.Entities.Identity;
using SmartHiring.Core.Services;
using SmartHiring.Repository.Data;
using SmartHiring.Service;
using System.Text;

namespace SmartHiring.APIs.Extensions
{
	public static class IdentityServicesExtension
	{
		public static IServiceCollection AddIdentityServices(this IServiceCollection Services,IConfiguration configuration)
		{
			Services.AddScoped<ITokenService, TokenService>();

			Services.AddIdentity<AppUser, IdentityRole>(options =>
			{
				options.Password.RequireDigit = true;
				options.Password.RequireLowercase = true;
				options.Password.RequireUppercase = true;
				options.Password.RequireNonAlphanumeric = true;
				options.Password.RequiredLength = 8;
			})
					.AddEntityFrameworkStores<SmartHiringDbContext>();

			Services.AddAuthentication(Options =>
			{
				Options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				Options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
				.AddJwtBearer(Options =>
				{
					Options.TokenValidationParameters = new TokenValidationParameters()
					{
						ValidateIssuer = true,
						ValidIssuer = configuration["JWT:ValidIssuer"],
						ValidateAudience = true,
						ValidAudience = configuration["JWT:ValidAudience"],
						ValidateLifetime = true,
						ValidateIssuerSigningKey = true,
						IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"])),
						ClockSkew = TimeSpan.Zero
					};
				});
					

			return Services;
		}
	}
}
