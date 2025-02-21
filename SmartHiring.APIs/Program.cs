using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartHiring.APIs.Errors;
using SmartHiring.APIs.Extensions;
using SmartHiring.APIs.Helpers;
using SmartHiring.APIs.Middlewares;
using SmartHiring.Core.Entities;
using SmartHiring.Core.Repositories;
using SmartHiring.Repository;
using SmartHiring.Repository.Data;

namespace SmartHiring.APIs
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			#region Configure Services - Add services to the container.

			builder.Services.AddControllers();
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();
			builder.Services.AddDbContext<SmartHiringContext>(Options =>
			{
				Options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
			});


			builder.Services.AddApplicationServices();


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
				var dbContext = Services.GetRequiredService<SmartHiringContext>();
				await dbContext.Database.MigrateAsync();
				await SmartHiringContextSeed.SeedAsync(dbContext);
			}
			catch (Exception ex)
			{
				var Logger = LoggerFactory.CreateLogger<Program>();
				Logger.LogError(ex, "An Error Occured During Appling The Migration");
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

			app.UseStaticFiles(); // CVs
			app.UseCors("MyPolicy");

			app.UseAuthorization();


			app.MapControllers();

			#endregion


			app.Run();
		}
	}
}
