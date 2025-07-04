﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartHiring.APIs.DTOs;
using SmartHiring.APIs.Errors;
using SmartHiring.APIs.Helpers;
using SmartHiring.Core;
using SmartHiring.Core.Entities;
using SmartHiring.Core.Entities.Identity;
using SmartHiring.Core.Specifications;
using SmartHiring.Repository.Data;

namespace SmartHiring.APIs.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : APIBaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly SmartHiringDbContext _dbContext;

        public AdminController(
            IUnitOfWork unitOfWork,
            UserManager<AppUser> userManager,
            IMapper mapper,
            SmartHiringDbContext dbContext)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
            _dbContext = dbContext;
        }

        #region Companies

        #region Get Companies
        [HttpGet("companies")]
        public async Task<ActionResult<IEnumerable<CompanyDto>>> GetCompanies([FromQuery] string? search)
        {
            var spec = new CompaniesWithDetailsSpec(search);
            var companies = await _unitOfWork.Repository<Company>().GetAllWithSpecAsync(spec);
            return Ok(_mapper.Map<IEnumerable<CompanyDto>>(companies));
        }
        #endregion

        #region Create Company
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

            await _unitOfWork.Repository<Company>().AddAsync(company);
            await _unitOfWork.CompleteAsync();

            return Ok(new ApiResponse(200, "Company created successfully"));
        }
        #endregion

        #region Update Company
        [HttpPut("companies/{companyId}")]
        public async Task<IActionResult> UpdateCompany(int companyId, [FromForm] UpdateCompanyByAdminDto dto)
        {
            var company = await _dbContext.Companies
                .Include(c => c.Manager)
                .FirstOrDefaultAsync(c => c.Id == companyId);

            if (company == null) return NotFound(new ApiResponse(404, "Company not found"));

            if (dto.Name != null)
                company.Name = dto.Name;

            if (dto.BusinessEmail != null)
                company.BusinessEmail = dto.BusinessEmail;

            if (dto.Phone != null)
                company.Phone = dto.Phone;

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

            await _unitOfWork.Repository<Company>().UpdateAsync(company);
            await _unitOfWork.CompleteAsync();
            return Ok(new ApiResponse(200, "Company updated successfully"));
        }
        #endregion

        #region Delete Company
        [HttpDelete("companies/{companyId}")]
        public async Task<IActionResult> DeleteCompany(int companyId)
        {
            try
            {
                // Use combination of UnitOfWork for basic operations and DbContext for complex queries
                var company = await _unitOfWork.Repository<Company>().GetByIdAsync(companyId);
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

                // Delete related entities using DbContext for bulk operations
                _dbContext.CandidateLists.RemoveRange(
                    _dbContext.CandidateLists.Where(c => applicationIds.Contains(c.PostId)));

                _dbContext.Interviews.RemoveRange(
                    _dbContext.Interviews.Where(i => postIds.Contains(i.PostId)));

                _dbContext.Applications.RemoveRange(
                    _dbContext.Applications.Where(a => postIds.Contains(a.PostId)));

                _dbContext.Posts.RemoveRange(
                    _dbContext.Posts.Where(p => postIds.Contains(p.Id)));

                // Delete company using UnitOfWork
                await _unitOfWork.Repository<Company>().DeleteAsync(company);

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

                await _unitOfWork.CompleteAsync();

                foreach (var user in usersToDelete)
                {
                    await _userManager.DeleteAsync(user);
                }

                return Ok(new ApiResponse(200, "Company and related data deleted successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"An error occurred: {ex.Message}"));
            }
        }
        #endregion

        #endregion

        #region Agencies

        #region Get Agencies
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
        #endregion

        #region Create Agency
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
        #endregion

        #region Update Agency
        [HttpPut("agencies/{agencyId}")]
        public async Task<IActionResult> UpdateAgency(string agencyId, [FromBody] UpdateAgencyByAdminDto dto)
        {
            var agency = await _userManager.FindByIdAsync(agencyId);
            if (agency == null) return NotFound(new ApiResponse(404, "Agency not found"));

            if (dto.Email != null && dto.Email != agency.Email)
            {
                var emailExists = await _userManager.Users.AnyAsync(a => a.Email == dto.Email && a.Id != agencyId);
                if (emailExists)
                    return BadRequest(new ApiResponse(400, "Agency with the same Email already exists."));
            }

            if (dto.PhoneNumber != null && dto.PhoneNumber != agency.PhoneNumber)
            {
                var phoneExists = await _userManager.Users.AnyAsync(a => a.PhoneNumber == dto.PhoneNumber && a.Id != agencyId);
                if (phoneExists)
                    return BadRequest(new ApiResponse(400, "Agency with the same Phone already exists."));
            }

            if (dto.AgencyName != null && dto.AgencyName != agency.AgencyName)
            {
                var nameExists = await _userManager.Users.AnyAsync(a => a.AgencyName == dto.AgencyName && a.Id != agencyId);
                if (nameExists)
                    return BadRequest(new ApiResponse(400, "Agency with the same AgencyName already exists."));
            }

            if (dto.AgencyName != null)
                agency.AgencyName = dto.AgencyName;

            if (dto.Email != null)
                agency.Email = dto.Email;

            if (dto.PhoneNumber != null)
                agency.PhoneNumber = dto.PhoneNumber;

            if (dto.Password != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(agency);
                await _userManager.ResetPasswordAsync(agency, token, dto.Password);
            }

            await _userManager.UpdateAsync(agency);
            return Ok(new ApiResponse(200, "Agency updated successfully"));
        }
        #endregion

        #region Delete Agency
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

        #endregion
    }
}