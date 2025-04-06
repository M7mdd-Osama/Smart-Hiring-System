using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartHiring.APIs.DTOs;
using SmartHiring.APIs.Errors;
using SmartHiring.Core.Entities.Identity;
using SmartHiring.Repository.Data;

namespace SmartHiring.APIs.Controllers
{
    public class UserController : APIBaseController
    {
        private readonly SmartHiringDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        private readonly PasswordHasher<AppUser> _passwordHasher;

        public UserController(SmartHiringDbContext dbContext,
            IMapper mapper,
            UserManager<AppUser> userManager,
            PasswordHasher<AppUser> passwordHasher)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _userManager = userManager;
            _passwordHasher = passwordHasher;
        }

        #region Edit Profile for Agency

        [Authorize(Roles = "Agency")]
        [HttpPut("Agency_edit")]
        public async Task<IActionResult> EditAgencyAccount([FromBody] EditAgencyDto request)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized(new ApiResponse(401, "User email not found in token"));

            var user = await _userManager.Users
                .Include(u => u.Address)
                .FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
                return Unauthorized(new ApiResponse(401, "User not found"));

            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Contains("Agency"))
                return Unauthorized(new ApiResponse(401, "This user is not an agency"));

            if (!string.IsNullOrEmpty(request.CurrentPassword))
            {
                var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.CurrentPassword);
                if (passwordVerificationResult == PasswordVerificationResult.Failed)
                {
                    return Unauthorized(new ApiResponse(401, "Invalid current password"));
                }
            }

            _mapper.Map(request, user);

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(new ApiResponse(400, "Failed to update user"));

            if (request.Address != null)
            {
                if (user.Address != null)
                {
                    user.Address.City = request.Address.City;
                    user.Address.Country = request.Address.Country;
                    _dbContext.Update(user.Address);
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
                    _dbContext.Add(newAddress);
                }

                await _dbContext.SaveChangesAsync();
            }

            return Ok(new { message = "Agency account and address updated successfully" });
        }

        #endregion


    }
}
