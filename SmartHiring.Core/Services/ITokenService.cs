using Microsoft.AspNetCore.Identity;
using SmartHiring.Core.Entities.Identity;

namespace SmartHiring.Core.Services
{
	public interface ITokenService
	{
		Task<string> CreateTokenAsync(AppUser User, UserManager<AppUser> userManager);
	}
}
