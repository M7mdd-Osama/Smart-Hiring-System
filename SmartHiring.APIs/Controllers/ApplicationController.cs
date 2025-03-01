using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartHiring.APIs.DTOs;
using SmartHiring.Core.Entities;
using SmartHiring.Core.Entities.Identity;
using SmartHiring.Core.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartHiring.APIs.Controllers
{
	public class ApplicationController : APIBaseController
	{
		private readonly IGenericRepository<Application> _applicationRepository;
		private readonly UserManager<AppUser> _userManager;

		public ApplicationController(
			IGenericRepository<Application> applicationRepository,
			UserManager<AppUser> userManager)
		{
			_applicationRepository = applicationRepository;
			_userManager = userManager;
		}

		[HttpPost]
		public async Task<IActionResult> CreateApplication([FromBody] ApplicationDto applicationDto)
		{
			if (applicationDto == null || string.IsNullOrEmpty(applicationDto.CV_Link))
				return BadRequest(new { message = "Invalid application data. CV_Link is required." });

			var agency = await _userManager.FindByIdAsync(applicationDto.AgencyId);
			if (agency == null)
				return NotFound(new { message = "Agency not found." });

			var application = new Application
			{
				RankScore = applicationDto.RankScore,
				IsShortlisted = applicationDto.IsShortlisted,
				ApplicationDate = DateTime.UtcNow,
				CV_Link = applicationDto.CV_Link,
				ApplicantId = applicationDto.ApplicantId,
				PostId = applicationDto.PostId,
				AgencyId = applicationDto.AgencyId
			};

			await _applicationRepository.AddAsync(application);

			return Ok(new { message = "Application submitted successfully", applicationId = application.Id });
		}

		[HttpGet("{applicationId}")]
		public async Task<ActionResult<ApplicationDto>> GetApplication(int applicationId)
		{
			var application = await _applicationRepository.GetByIdAsync(applicationId);
			if (application == null)
				return NotFound();

			var applicationDto = new ApplicationDto
			{
				Id = application.Id,
				RankScore = application.RankScore,
				IsShortlisted = application.IsShortlisted,
				ApplicationDate = application.ApplicationDate,
				CV_Link = application.CV_Link,
				ApplicantId = application.ApplicantId,
				PostId = application.PostId,
				AgencyId = application.AgencyId
			};

			return Ok(applicationDto);
		}

		[HttpGet("job/{jobId}")]
		public async Task<ActionResult<IEnumerable<ApplicationDto>>> GetApplicationsForJob(int jobId)
		{
			var applications = await _applicationRepository.GetAllAsync();
			var filteredApplications = applications
				.Where(a => a.PostId == jobId)
				.Select(a => new ApplicationDto
				{
					Id = a.Id,
					RankScore = a.RankScore,
					IsShortlisted = a.IsShortlisted,
					ApplicationDate = a.ApplicationDate,
					CV_Link = a.CV_Link,
					ApplicantId = a.ApplicantId,
					PostId = a.PostId,
					AgencyId = a.AgencyId
				});

			return Ok(filteredApplications);
		}

		[HttpPut("{applicationId}/approve")]
		public async Task<IActionResult> ApproveApplication(int applicationId)
		{
			var application = await _applicationRepository.GetByIdAsync(applicationId);
			if (application == null)
				return NotFound();

			application.IsShortlisted = true;
			await _applicationRepository.UpdateAsync(application);

			return Ok(new { message = "Application approved successfully", applicationId });
		}

		[HttpPut("{applicationId}/reject")]
		public async Task<IActionResult> RejectApplication(int applicationId)
		{
			var application = await _applicationRepository.GetByIdAsync(applicationId);
			if (application == null)
				return NotFound();

			application.IsShortlisted = false;
			await _applicationRepository.UpdateAsync(application);

			return Ok(new { message = "Application rejected successfully", applicationId });
		}

		[HttpGet("{applicationId}/status")]
		public async Task<IActionResult> GetApplicationStatus(int applicationId)
		{
			var application = await _applicationRepository.GetByIdAsync(applicationId);
			if (application == null)
				return NotFound();

			return Ok(new { applicationId, status = application.IsShortlisted ? "Approved" : "Rejected" });
		}
	}
}