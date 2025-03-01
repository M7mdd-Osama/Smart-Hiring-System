using Microsoft.AspNetCore.Identity;
using SmartHiring.Core.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHiring.Core.Services
{
	public interface ITokenService
	{
		Task<string> CreateTokenAsync(AppUser User, UserManager<AppUser> userManager);
	}
}
