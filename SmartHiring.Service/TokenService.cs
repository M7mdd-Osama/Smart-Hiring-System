﻿using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SmartHiring.Core.Entities.Identity;
using SmartHiring.Core.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SmartHiring.Service
{
	public class TokenService : ITokenService
	{
		private readonly IConfiguration _configuration;

		public TokenService(IConfiguration configuration)
		{
			_configuration = configuration;
		}
		public async Task<string> CreateTokenAsync(AppUser User, UserManager<AppUser> userManager)
		{
			var AuthClaims = new List<Claim>()
			{
				new Claim(ClaimTypes.GivenName, User.DisplayName),
				new Claim(ClaimTypes.Email, User.Email)
			};
			var UserRoles = await userManager.GetRolesAsync(User);
			foreach (var Role in UserRoles)
			{
				AuthClaims.Add(new Claim(ClaimTypes.Role, Role));
			}
			var AuthKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));

			var Token = new JwtSecurityToken(
				issuer: _configuration["JWT:ValidIssuer"],
				audience: _configuration["JWT:ValidAudience"],
				expires: DateTime.UtcNow.AddDays(double.Parse(_configuration["JWT:DurationInDays"])),
				claims: AuthClaims,
				signingCredentials: new SigningCredentials(AuthKey, SecurityAlgorithms.HmacSha256Signature)
				);
			return new JwtSecurityTokenHandler().WriteToken(Token);
		}
	}
}
