using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartHiring.APIs.Extensions;
using SmartHiring.APIs.Middlewares;
using SmartHiring.APIs.Settings;
using SmartHiring.Core.Entities.Identity;
using SmartHiring.Repository.Data;

namespace SmartHiring.APIs
{
	public class Program
	{
		public static async Task Main(string[] args)
		{

			var builder = WebApplication.CreateBuilder(args);

			#region Configure Services - Add services to the container.

			builder.Services.AddControllers()
			 .AddJsonOptions(options =>
			 {
				 options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
				 options.JsonSerializerOptions.WriteIndented = true;
				 options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
			 });

			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			builder.Services.AddDbContext<SmartHiringDbContext>(Options =>
				Options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), sqlOptions =>
				{
					sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
				}));

			builder.Services.AddApplicationServices();
			builder.Services.AddIdentityServices(builder.Configuration);

			builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

			builder.Services.AddCors(Options =>
			{
				Options.AddPolicy("MyPolicy", options =>
				{
					options.AllowAnyHeader();
					options.AllowAnyMethod();
					options.AllowAnyOrigin();
				});
			});

			#endregion

			var app = builder.Build();


			#region Update-Database

			using var Scope = app.Services.CreateScope();
			var Services = Scope.ServiceProvider;

			var LoggerFactory = Services.GetRequiredService<ILoggerFactory>();
			try
			{
				var dbContext = Services.GetRequiredService<SmartHiringDbContext>();
				await dbContext.Database.MigrateAsync();

				var UserManager = Services.GetRequiredService<UserManager<AppUser>>();
				var RoleManager = Services.GetRequiredService<RoleManager<IdentityRole>>();
				await AppIdentitySmartHiringContextSeed.SeedUserAsync(UserManager, RoleManager);
				//await SmartHiringContextSeed.SeedAsync(dbContext);
			}
			catch (Exception ex)
			{
				var Logger = LoggerFactory.CreateLogger<Program>();
				Logger.LogError(ex, "An Error Occurred During Applying The Migration");
			}

			#endregion

			#region Configure - Configure the HTTP request pipeline.

			app.UseMiddleware<ExceptionMiddleware>();
			if (app.Environment.IsDevelopment())
			{
				app.UseSwaggerMiddlewares();
			}
			app.UseStatusCodePagesWithReExecute("/errors/{0}");
			app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.UseCors("MyPolicy");

			app.UseAuthentication();
			app.UseAuthorization();

			app.MapControllers();

			#endregion

			app.Run();
		}
	}
}