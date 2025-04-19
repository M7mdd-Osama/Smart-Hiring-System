using System.Security.Claims;
using AutoMapper;
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

namespace SmartHiring.APIs.Controllers
{
    public class UserController : APIBaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;

        public UserController(IUnitOfWork unitOfWork,
            UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        #region Edit Profile for HR, Manager, Agency, and Admin

        [Authorize(Roles = "HR,Manager,Agency,Admin")]
        [HttpPut("edit-profile")]
        public async Task<IActionResult> EditUserAccount([FromForm] EditUserDto request)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized(new ApiResponse(401, "User email not found in token"));

            var user = await _userManager.Users
                .Include(u => u.Address)
                .Include(u => u.HRCompany)
                .Include(u => u.ManagedCompany)
                .FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
                return Unauthorized(new ApiResponse(401, "User not found"));

            var roles = await _userManager.GetRolesAsync(user);

            if (!(roles.Contains("HR") || roles.Contains("Manager") || roles.Contains("Agency") || roles.Contains("Admin")))
                return Unauthorized(new ApiResponse(401, "This user does not have permission"));

            if (!string.IsNullOrEmpty(request.NewPassword))
            {
                var passwordValidationResult = await _userManager.RemovePasswordAsync(user);
                if (!passwordValidationResult.Succeeded)
                {
                    return BadRequest(new ApiResponse(400, "Failed to update password"));
                }

                var addPasswordResult = await _userManager.AddPasswordAsync(user, request.NewPassword);
                if (!addPasswordResult.Succeeded)
                {
                    return BadRequest(new ApiResponse(400, "Failed to update password"));
                }
            }

            if (request.FirstName != null)
                user.FirstName = request.FirstName;

            if (request.LastName != null)
                user.LastName = request.LastName;

            if (request.AgencyName != null)
                user.AgencyName = request.AgencyName;

            if (!string.IsNullOrEmpty(request.Email))
                user.Email = request.Email;

            if (!string.IsNullOrEmpty(request.PhoneNumber))
                user.PhoneNumber = request.PhoneNumber;

            if (roles.Contains("Manager"))
            {
                if (request.CompanyLogo != null)
                {
                    var company = user.ManagedCompany;
                    if (company != null)
                    {
                        string logoPath = null;
                        if (request.CompanyLogo != null)
                        {
                            var uploadedFileName = DocumentSettings.UploadFile(request.CompanyLogo, "Images");
                            logoPath = $"/Files/Images/{uploadedFileName}";
                        }
                        company.LogoUrl = logoPath;
                        await _unitOfWork.Repository<Company>().UpdateAsync(company);
                    }
                    else
                    {
                        return BadRequest(new ApiResponse(400, "No associated company found for the Manager"));
                    }
                }
            }
            else
            {
                if (request.CompanyLogo != null)
                {
                    return BadRequest(new ApiResponse(400, "Only a Manager can change the company logo"));
                }
            }

            if (request.Address != null)
            {
                if (user.Address != null)
                {
                    user.Address.City = request.Address.City ?? user.Address.City;
                    user.Address.Country = request.Address.Country ?? user.Address.Country;
                    await _unitOfWork.Repository<Address>().UpdateAsync(user.Address);
                }
                else
                {
                    var newAddress = new Address
                    {
                        AppUserId = user.Id,
                        City = request.Address.City,
                        Country = request.Address.Country
                    };
                    user.Address = newAddress;
                    await _unitOfWork.Repository<Address>().AddAsync(newAddress);
                }
            }

            await _unitOfWork.CompleteAsync();

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(new ApiResponse(400, "Failed to update user"));

            return Ok(new { message = "User account and address updated successfully" });
        }

        #endregion
    }
}