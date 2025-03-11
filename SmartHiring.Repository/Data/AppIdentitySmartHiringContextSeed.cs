using Microsoft.AspNetCore.Identity;
using SmartHiring.Core.Entities.Identity;
using System.Linq;
using System.Threading.Tasks;

namespace SmartHiring.Repository.Data
{
	public static class AppIdentitySmartHiringContextSeed
	{
		public static async Task SeedUserAsync(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
		{
			//string[] roles = new string[] { "Admin", "Manager", "HR", "Agency" };
			//foreach (var role in roles)
			//{
			//	if (!await roleManager.RoleExistsAsync(role))
			//	{
			//		await roleManager.CreateAsync(new IdentityRole(role));
			//	}
			//}

			//if (!userManager.Users.Any(u => u.UserName == "mohamedosama"))
			//{
			//	var adminUser = new AppUser()
			//	{
			//		DisplayName = "Angelo",
			//		Email = "mohamedosama@gmail.com",
			//		UserName = "mohamedosama",
			//		PhoneNumber = "01203893166",
			//		EmailConfirmed = true,
			//	};

			//	var result = await userManager.CreateAsync(adminUser, "Pa$$w0rd");

			//	if (result.Succeeded)
			//	{
			//		await userManager.AddToRoleAsync(adminUser, "Admin");
			//	}
			//}

			//if (!userManager.Users.Any(u => u.UserName == "manageruser"))
			//{
			//	var managerUser = new AppUser()
			//	{
			//		DisplayName = "Manager Mike",
			//		Email = "manager@gmail.com",
			//		UserName = "manageruser",
			//		PhoneNumber = "01012345678",
			//		EmailConfirmed = true,
			//	};

			//	var result = await userManager.CreateAsync(managerUser, "Manager@123");

			//	if (result.Succeeded)
			//	{
			//		await userManager.AddToRoleAsync(managerUser, "Manager");
			//	}
			//}

			//if (!userManager.Users.Any(u => u.UserName == "hruser"))
			//{
			//	var hrUser = new AppUser()
			//	{
			//		DisplayName = "HR Sarah",
			//		Email = "hr@gmail.com",
			//		UserName = "hruser",
			//		PhoneNumber = "01122334455",
			//		EmailConfirmed = true,
			//	};

			//	var result = await userManager.CreateAsync(hrUser, "Hr@123");

			//	if (result.Succeeded)
			//	{
			//		await userManager.AddToRoleAsync(hrUser, "HR");
			//	}
			//}

			//if (!userManager.Users.Any(u => u.UserName == "agencyuser"))
			//{
			//	var agencyUser = new AppUser()
			//	{
			//		DisplayName = "Agency Alice",
			//		Email = "agency@gmail.com",
			//		UserName = "agencyuser",
			//		PhoneNumber = "01555667788",
			//		EmailConfirmed = true,
			//	};

			//	var result = await userManager.CreateAsync(agencyUser, "Agency@123");

			//	if (result.Succeeded)
			//	{
			//		await userManager.AddToRoleAsync(agencyUser, "Agency");
			//	}
			//}
		}
	}
}