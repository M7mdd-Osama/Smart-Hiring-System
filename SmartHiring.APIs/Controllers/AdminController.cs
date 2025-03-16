using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartHiring.APIs.DTOs;
using SmartHiring.APIs.Errors;
using SmartHiring.Core.Entities;
using SmartHiring.Core.Entities.Identity;
using SmartHiring.Core.Repositories;
using SmartHiring.Core.Specifications;
using SmartHiring.Repository.Data;

namespace SmartHiring.APIs.Controllers
{
	[Authorize(Roles = "Admin")]
	public class AdminController : APIBaseController
	{
		private readonly IGenericRepository<Company> _companyRepo;
		private readonly UserManager<AppUser> _userManager;
		private readonly IMapper _mapper;
		private readonly SmartHiringDbContext _dbContext;

		public AdminController(
			IGenericRepository<Company> companyRepo,
			UserManager<AppUser> userManager,
			IMapper mapper,
			SmartHiringDbContext dbContext)
		{
			_companyRepo = companyRepo;
			_userManager = userManager;
			_mapper = mapper;
			_dbContext = dbContext;
		}

		#region Companies

		[HttpGet("companies")]
		public async Task<ActionResult<IEnumerable<CompanyDto>>> GetCompanies()
		{
			var spec = new CompaniesWithDetailsSpecification();
			var companies = await _companyRepo.GetAllWithSpecAsync(spec);
			return Ok(_mapper.Map<IEnumerable<CompanyDto>>(companies));
		}

		[HttpDelete("companies/{companyId}")]
		public async Task<IActionResult> DeleteCompany(int companyId)
		{
			try
			{
				var company = await _dbContext.Companies.FindAsync(companyId);
				if (company == null)
				{
					return NotFound(new ApiResponse(404, "Company not found"));
				}

				var postIds = await _dbContext.Posts
					.Where(p => p.CompanyId == companyId)
					.Select(p => p.Id)
					.ToListAsync();

				var applicationIds = await _dbContext.Applications
					.Where(a => postIds.Contains(a.PostId))
					.Select(a => a.Id)
					.ToListAsync();

				_dbContext.CandidateLists.RemoveRange(_dbContext.CandidateLists.Where(c => applicationIds.Contains(c.PostId)));

				_dbContext.Interviews.RemoveRange(_dbContext.Interviews.Where(i => postIds.Contains(i.PostId)));

				_dbContext.Applications.RemoveRange(_dbContext.Applications.Where(a => postIds.Contains(a.PostId)));

				_dbContext.Posts.RemoveRange(_dbContext.Posts.Where(p => postIds.Contains(p.Id)));

				_dbContext.CompanyPhones.RemoveRange(_dbContext.CompanyPhones.Where(cp => cp.CompanyId == companyId));

				_dbContext.Companies.Remove(company);

				var usersToDelete = new List<AppUser>();

				if (!string.IsNullOrEmpty(company.HRId))
				{
					bool hrHasOtherCompanies = await _dbContext.Companies
						.AnyAsync(c => c.HRId == company.HRId && c.Id != companyId);

					if (!hrHasOtherCompanies)
					{
						var hr = await _userManager.FindByIdAsync(company.HRId);
						if (hr != null) usersToDelete.Add(hr);
					}
				}

				if (!string.IsNullOrEmpty(company.ManagerId))
				{
					bool managerHasOtherCompanies = await _dbContext.Companies
						.AnyAsync(c => c.ManagerId == company.ManagerId && c.Id != companyId);

					if (!managerHasOtherCompanies)
					{
						var manager = await _userManager.FindByIdAsync(company.ManagerId);
						if (manager != null) usersToDelete.Add(manager);
					}
				}

				foreach (var user in usersToDelete)
				{
					await _userManager.DeleteAsync(user);
				}

				await _dbContext.SaveChangesAsync();

				return Ok(new ApiResponse(200, "Company and related data deleted successfully"));
			}
			catch (Exception ex)
			{
				return StatusCode(500, new ApiResponse(500, $"An error occurred: {ex.Message}"));
			}
		}

		#endregion

		#region Agencies

		[HttpGet("agencies")]
		public async Task<ActionResult<IEnumerable<AgencyDto>>> GetAgencies()
		{
			var agencies = await _userManager.GetUsersInRoleAsync("Agency");
			return Ok(_mapper.Map<IEnumerable<AgencyDto>>(agencies));
		}

		[HttpDelete("agencies/{agencyId}")]
		public async Task<IActionResult> DeleteAgency(string agencyId)
		{
			var agency = await _userManager.FindByIdAsync(agencyId);
			if (agency == null)
				return NotFound(new ApiResponse(404, "Agency not found."));

			await _userManager.DeleteAsync(agency);
			return Ok(new ApiResponse(200, $"Agency has been deleted."));
		}

		#endregion

	}
}