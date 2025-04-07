using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartHiring.APIs.DTOs;
using SmartHiring.APIs.Errors;
using SmartHiring.APIs.Helpers;
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
        public async Task<ActionResult<IEnumerable<CompanyDto>>> GetCompanies([FromQuery] string? search)
        {
            var spec = new CompaniesWithDetailsSpecification(search);
            var companies = await _companyRepo.GetAllWithSpecAsync(spec);
            return Ok(_mapper.Map<IEnumerable<CompanyDto>>(companies));
        }

        [HttpPost("companies")]
        public async Task<IActionResult> CreateCompany([FromForm] CreateCompanyByAdminDto dto)
        {
            var existingCompany = await _dbContext.Companies
                .FirstOrDefaultAsync(c => c.Name == dto.Name || c.BusinessEmail == dto.BusinessEmail || c.Phone == dto.Phone);

            if (existingCompany != null)
            {
                return BadRequest(new ApiResponse(400, "Company with the same Name, Email, or Phone already exists."));
            }

            var company = _mapper.Map<Company>(dto);
            var passwordHasher = new PasswordHasher<Company>();
            company.Password = passwordHasher.HashPassword(company, dto.Password);

            if (dto.Logo != null)
            {
                string logoUrl = DocumentSettings.UploadFile(dto.Logo, "Images");
                company.LogoUrl = $"/Files/Images/{logoUrl}";
            }

            await _dbContext.Companies.AddAsync(company);
            await _dbContext.SaveChangesAsync();

            return Ok(new ApiResponse(200, "Company created successfully"));
        }

        [HttpPut("companies/{companyId}")]
        public async Task<IActionResult> UpdateCompany(int companyId, [FromForm] UpdateCompanyByAdminDto dto)
        {
            var company = await _dbContext.Companies.Include(c => c.Manager).FirstOrDefaultAsync(c => c.Id == companyId);
            if (company == null) return NotFound(new ApiResponse(404, "Company not found"));

            company = _mapper.Map(dto, company);

            if (dto.Password != null)
            {
                var passwordHasher = new PasswordHasher<Company>();
                company.Password = passwordHasher.HashPassword(company, dto.Password);
            }

            if (dto.Logo != null)
            {
                string logoUrl = DocumentSettings.UploadFile(dto.Logo, "Images");
                company.LogoUrl = $"/Files/Images/{logoUrl}";
            }

            await _dbContext.SaveChangesAsync();
            return Ok(new ApiResponse(200, "Company updated successfully"));
        }

        [HttpDelete("companies/{companyId}")]
        public async Task<IActionResult> DeleteCompany(int companyId)
        {
            try
            {
                var company = await _dbContext.Companies.FindAsync(companyId);
                if (company == null)
                    return NotFound(new ApiResponse(404, "Company not found"));

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
        public async Task<ActionResult<IEnumerable<AgencyDto>>> GetAgencies([FromQuery] string? search)
        {
            var agencies = await _userManager.GetUsersInRoleAsync("Agency");

            if (!string.IsNullOrEmpty(search))
            {
                agencies = agencies.Where(a => a.UserName.ToLower().Contains(search.ToLower())).ToList();
            }
            return Ok(_mapper.Map<IEnumerable<AgencyDto>>(agencies));
        }

        [HttpPost("agencies")]
        public async Task<IActionResult> CreateAgency([FromBody] CreateAgencyByAdminDto dto)
        {
            var existingAgency = await _userManager.Users
                .FirstOrDefaultAsync(a => a.Email == dto.Email || a.PhoneNumber == dto.PhoneNumber || a.AgencyName == dto.AgencyName);

            if (existingAgency != null)
            {
                return BadRequest(new ApiResponse(400, "Agency with the same Email, Phone, or AgencyName already exists."));
            }

            var user = new AppUser
            {
                UserName = dto.Email.Split('@')[0],
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                AgencyName = dto.AgencyName,
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                LockoutEnabled = false
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                string errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest(new ApiResponse(400, $"Failed to create Agency: {errors}"));
            }

            await _userManager.AddToRoleAsync(user, "Agency");

            return Ok(new ApiResponse(200, "Agency created successfully"));
        }

        [HttpPut("agencies/{agencyId}")]
        public async Task<IActionResult> UpdateAgency(string agencyId, [FromBody] UpdateAgencyByAdminDto dto)
        {
            var existingAgency = await _userManager.Users
                 .FirstOrDefaultAsync(a => a.Email == dto.Email || a.PhoneNumber == dto.PhoneNumber || a.AgencyName == dto.AgencyName);

            if (existingAgency != null)
            {
                return BadRequest(new ApiResponse(400, "Agency with the same Email, Phone, or AgencyName already exists."));
            }

            var agency = await _userManager.FindByIdAsync(agencyId);
            if (agency == null) return NotFound(new ApiResponse(404, "Agency not found"));

            agency = _mapper.Map(dto, agency);

            if (dto.Password != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(agency);
                await _userManager.ResetPasswordAsync(agency, token, dto.Password);
            }

            await _userManager.UpdateAsync(agency);
            return Ok(new ApiResponse(200, "Agency updated successfully"));
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