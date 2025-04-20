using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartHiring.APIs.DTOs;
using SmartHiring.APIs.Errors;
using SmartHiring.APIs.Helpers;
using SmartHiring.Core.Entities;
using SmartHiring.Core.Entities.Identity;
using SmartHiring.Core.Services;
using SmartHiring.Repository.Data;

namespace SmartHiring.APIs.Controllers
{
	public class AccountsController : APIBaseController
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly SmartHiringDbContext _dbContext;
		private readonly ImailSettings _mailSettings;
		private readonly IPasswordHasher<Company> _passwordHasher;
		private readonly SignInManager<AppUser> _signInManager;
		private readonly ITokenService _tokenService;

		public AccountsController(UserManager<AppUser> userManager,
			RoleManager<IdentityRole> roleManager,
			SmartHiringDbContext dbContext,
			ImailSettings mailSettings,
			IPasswordHasher<Company> passwordHasher,
			SignInManager<AppUser> signInManager,
			ITokenService tokenService)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_dbContext = dbContext;
			_mailSettings = mailSettings;
			_passwordHasher = passwordHasher;
			_signInManager = signInManager;
			_tokenService = tokenService;
		}

        #region Register

        #region RegisterCompany

        [HttpPost("RegisterCompany")]
		public async Task<ActionResult> RegisterCompany([FromForm] RegisterCompanyDto model)
		{
			if (await _dbContext.Companies.AnyAsync(c => c.Name == model.CompanyName))
				return BadRequest(new ApiResponse(400, "Company name already exists"));

			if (await _dbContext.Companies.AnyAsync(c => c.BusinessEmail == model.Email))
				return BadRequest(new ApiResponse(400, "Company email already exists"));

			if (await _dbContext.Companies.AnyAsync(p => p.Phone == model.PhoneNumber))
				return BadRequest(new ApiResponse(400, "Phone number already exists"));

			string logoPath = null;
			if (model.CompanyLogoUrl != null)
			{
				var uploadedFileName = DocumentSettings.UploadFile(model.CompanyLogoUrl, "Images");
				logoPath = $"/Files/Images/{uploadedFileName}";
			}

			var company = new Company
			{
				Name = model.CompanyName,
				BusinessEmail = model.Email,
				Phone = model.PhoneNumber,
				Password = _passwordHasher.HashPassword(null, model.Password),
				LogoUrl = logoPath,
				EmailConfirmed = false,
				ConfirmationCode = AuthHelper.GenerateOTP(),
				ConfirmationCodeExpires = DateTime.UtcNow.AddMinutes(10),
				CreatedAt = DateTime.UtcNow
			};

			_dbContext.Companies.Add(company);
			await _dbContext.SaveChangesAsync();

			await _dbContext.SaveChangesAsync();

			await AuthHelper.SendConfirmationEmail(_mailSettings, company.BusinessEmail, company.ConfirmationCode);

			return Ok(new ApiResponse(200, "Company registered successfully. Please confirm your email."));
		}

        #endregion

        #region RegisterUsers
        [HttpPost("RegisterUsers")]
		public async Task<ActionResult<UserDto>> Register(RegisterDto model)
		{
			if (model.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
				return BadRequest(new ApiResponse(400, "Admin cannot be registered through this endpoint"));

			if (!await _roleManager.RoleExistsAsync(model.Role))
				return BadRequest(new ApiResponse(400, "Invalid role"));

			if (await _userManager.FindByEmailAsync(model.Email) != null)
				return BadRequest(new ApiResponse(400, "Email already exists"));

			if (await _userManager.Users.AnyAsync(u => u.PhoneNumber == model.PhoneNumber))
				return BadRequest(new ApiResponse(400, "Phone number already exists"));

			var user = new AppUser
			{
				Id = Guid.NewGuid().ToString(),
				FirstName = model.FirstName,
				LastName = model.LastName,
				Email = model.Email,
				UserName = model.Email.Split('@')[0],
				PhoneNumber = model.PhoneNumber,
				EmailConfirmed = false,
				ConfirmationCode = AuthHelper.GenerateOTP(),
				ConfirmationCodeExpires = DateTime.UtcNow.AddMinutes(10),
				CreatedAt = DateTime.UtcNow
			};

			if (model.Role.Equals("HR", StringComparison.OrdinalIgnoreCase) || model.Role.Equals("Manager", StringComparison.OrdinalIgnoreCase))
			{
				var company = await _dbContext.Companies.FirstOrDefaultAsync(c => c.BusinessEmail == model.CompanyEmail);
				if (company == null || _passwordHasher.VerifyHashedPassword(company, company.Password, model.CompanyPassword) != PasswordVerificationResult.Success)
					return BadRequest(new ApiResponse(400, "Invalid company credentials"));

				if (model.Role.Equals("HR", StringComparison.OrdinalIgnoreCase) && company.HRId != null)
					return BadRequest(new ApiResponse(400, "HR already exists for this company"));

				if (model.Role.Equals("Manager", StringComparison.OrdinalIgnoreCase) && company.ManagerId != null)
					return BadRequest(new ApiResponse(400, "Manager already exists for this company"));

				var result = await _userManager.CreateAsync(user, model.Password);
				if (!result.Succeeded)
					return BadRequest(new ApiResponse(400, $"Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}"));

				if (model.Role.Equals("HR", StringComparison.OrdinalIgnoreCase))
					company.HRId = user.Id;
				else if (model.Role.Equals("Manager", StringComparison.OrdinalIgnoreCase))
					company.ManagerId = user.Id;

				await _dbContext.SaveChangesAsync();
			}
			else if (model.Role.Equals("Agency", StringComparison.OrdinalIgnoreCase))
			{
				if (await _userManager.Users.AnyAsync(u => u.AgencyName == model.AgencyName))
					return BadRequest(new ApiResponse(400, "Agency name already exists"));

				user.AgencyName = model.AgencyName;

				var result = await _userManager.CreateAsync(user, model.Password);
				if (!result.Succeeded)
					return BadRequest(new ApiResponse(400, $"Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}"));
			}

			await _userManager.AddToRoleAsync(user, model.Role);
			await _dbContext.SaveChangesAsync();

			await AuthHelper.SendConfirmationEmail(_mailSettings, user.Email, user.ConfirmationCode);

			return Ok(new ApiResponse(200, "User registered successfully. Please confirm your email."));
		}
        #endregion

        #region ConfirmEmail
        [HttpPost("ConfirmEmail")]
		public async Task<ActionResult<UserDto>> ConfirmEmail(ConfirmEmailDto model)
		{
			var user = await _userManager.Users
				.Include(u => u.HRCompany)
				.Include(u => u.ManagedCompany)
				.FirstOrDefaultAsync(u => u.Email == model.Email);

			if (user != null)
			{
				if (user.EmailConfirmed)
					return BadRequest(new ApiResponse(400, "Email already confirmed"));

				if (user.ConfirmationCodeExpires < DateTime.UtcNow)
					return BadRequest(new ApiResponse(400, "OTP expired"));

				if (user.ConfirmationCode != model.OTP)
					return BadRequest(new ApiResponse(400, "Invalid OTP"));

				user.EmailConfirmed = true;
				user.ConfirmationCode = null;
				user.ConfirmationCodeExpires = null;
				await _userManager.UpdateAsync(user);

				string companyLogo = null;
				if (await _userManager.IsInRoleAsync(user, "HR") && user.HRCompany != null)
				{
					companyLogo = user.HRCompany.LogoUrl;
				}
				else if (await _userManager.IsInRoleAsync(user, "Manager") && user.ManagedCompany != null)
				{
					companyLogo = user.ManagedCompany.LogoUrl;
				}

				return Ok(new
				{
					DisplayName = !string.IsNullOrEmpty(user.FirstName) && !string.IsNullOrEmpty(user.LastName)
						? $"{user.FirstName} {user.LastName}"
						: user.AgencyName ?? "N/A",
					user.Email,
					Token = await _tokenService.CreateTokenAsync(user, _userManager),
					CompanyLogoUrl = companyLogo
				});
			}

			var company = await _dbContext.Companies.FirstOrDefaultAsync(c => c.BusinessEmail == model.Email);

			if (company == null)
				return BadRequest(new ApiResponse(400, "Invalid email"));

			if (company.EmailConfirmed)
				return BadRequest(new ApiResponse(400, "Email already confirmed"));

			if (company.ConfirmationCodeExpires < DateTime.UtcNow)
				return BadRequest(new ApiResponse(400, "OTP expired"));

			if (company.ConfirmationCode != model.OTP)
				return BadRequest(new ApiResponse(400, "Invalid OTP"));

			company.EmailConfirmed = true;
			company.ConfirmationCode = null;
			company.ConfirmationCodeExpires = null;
			await _dbContext.SaveChangesAsync();

			return Ok(new ApiResponse(200, "Email confirmed successfully."));
		}

        #endregion

        #region ResendOTP
        [HttpPost("ResendOTP")]
		public async Task<IActionResult> ResendOTP(string email)
		{
			var user = await _userManager.FindByEmailAsync(email);
			if (user == null || user.EmailConfirmed)
				return NotFound(new ApiResponse(404, "Not found or already confirmed"));

			var otp = AuthHelper.GenerateOTP();
			user.ConfirmationCode = otp;
			user.ConfirmationCodeExpires = DateTime.UtcNow.AddMinutes(10);

			await _userManager.UpdateAsync(user);
			await AuthHelper.SendConfirmationEmail(_mailSettings, user.Email, otp);

			return Ok(new ApiResponse(200, "New OTP has been sent"));
		}

		#endregion

		#endregion

		#region Login

		[HttpPost("Login")]
		public async Task<ActionResult<UserDto>> Login(LoginDto model)
		{
			var user = await _userManager.Users
				.Include(u => u.HRCompany)
				.Include(u => u.ManagedCompany)
				.FirstOrDefaultAsync(u => u.Email == model.Email);

			if (user == null)
				return Unauthorized(new ApiResponse(401, "Invalid email or password"));

			if (!user.EmailConfirmed)
				return Unauthorized(new ApiResponse(401, "Email not confirmed. Please verify your email."));

			var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
			if (!result.Succeeded)
				return Unauthorized(new ApiResponse(401, "Invalid email or password"));

			string companyLogo = null;

			if (await _userManager.IsInRoleAsync(user, "HR") && user.HRCompany != null)
			{
				companyLogo = user.HRCompany.LogoUrl;
			}
			else if (await _userManager.IsInRoleAsync(user, "Manager") && user.ManagedCompany != null)
			{
				companyLogo = user.ManagedCompany.LogoUrl;
			}

            return Ok(new
            {
                DisplayName = !string.IsNullOrEmpty(user.FirstName) && !string.IsNullOrEmpty(user.LastName)
                    ? $"{user.FirstName} {user.LastName}"
                    : user.AgencyName ?? "N/A",
                user.Email,
                Token = await _tokenService.CreateTokenAsync(user, _userManager),
                CompanyLogoUrl = companyLogo
            });
        }

        #endregion

        #region Forgot/Reset Password

        #region Forgot Password
        [HttpPost("ForgotPassword")]
		public async Task<IActionResult> ForgotPassword(ForgotPasswordDto model)
		{
			var user = await _userManager.FindByEmailAsync(model.Email);
			if (user == null)
				return BadRequest(new ApiResponse(400, "User not found"));

			var otp = AuthHelper.GenerateOTP();
			user.ConfirmationCode = otp;
			user.ConfirmationCodeExpires = DateTime.UtcNow.AddMinutes(10);

			await _userManager.UpdateAsync(user);
			await AuthHelper.SendConfirmationEmail(_mailSettings, user.Email, otp);

			return Ok(new ApiResponse(200, "OTP has been sent to your email."));
		}

        #endregion

        #region Reset Password
        [HttpPost("ResetPassword")]
		public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
		{
			var user = await _userManager.FindByEmailAsync(model.Email);
			if (user == null) return BadRequest(new ApiResponse(400, "User not found"));

			if (user.ConfirmationCodeExpires < DateTime.UtcNow || user.ConfirmationCode != model.Otp)
				return BadRequest(new ApiResponse(400, "Invalid or expired OTP"));

			var passwordCheck = _userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, model.NewPassword);
			if (passwordCheck == PasswordVerificationResult.Success)
				return BadRequest(new ApiResponse(400, "New password cannot be the same as the old password"));

			var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
			var result = await _userManager.ResetPasswordAsync(user, resetToken, model.NewPassword);

			if (!result.Succeeded)
				return BadRequest(new ApiResponse(400, $"Failed to reset password: " +
					$"{string.Join(", ", result.Errors.Select(e => e.Description))}"));

			user.ConfirmationCode = null;
			user.ConfirmationCodeExpires = null;
			await _userManager.UpdateAsync(user);

			return Ok(new ApiResponse(200, "Password has been reset successfully."));
		}

		#endregion

		#endregion

		#region Logout
		[Authorize]
		[HttpPost("Logout")]
		public async Task<IActionResult> Logout()
		{
			await _signInManager.SignOutAsync();
			return Ok(new ApiResponse(200, "Logged out successfully."));
		}
		#endregion

	}
}