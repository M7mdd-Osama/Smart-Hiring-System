using SmartHiring.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SmartHiring.Repository.Data
{
	public static class SmartHiringDbContextSeed
	{
		public static async Task SeedAsync(SmartHiringDbContext dbContext)
		{

		//	if (!dbContext.Companies.Any())
		//	{
		//		var CompaniesData = File.ReadAllText("../SmartHiring.Repository/Data/DataSeed/Companies.json");
		//		var Companies = JsonSerializer.Deserialize<List<Company>>(CompaniesData);
		//		if (Companies?.Count > 0)
		//		{
		//			foreach (var Company in Companies)
		//				await dbContext.Set<Company>().AddAsync(Company);
		//			await dbContext.SaveChangesAsync();
		//		}
		//	}



		//	if (!dbContext.Posts.Any())
		//	{
		//		var PostsData = File.ReadAllText("../SmartHiring.Repository/Data/DataSeed/Posts.json");
		//		var Posts = JsonSerializer.Deserialize<List<Post>>(PostsData);
		//		if (Posts?.Count > 0)
		//		{
		//			foreach (var Post in Posts)
		//				await dbContext.Set<Post>().AddAsync(Post);
		//			await dbContext.SaveChangesAsync();
		//		}
		//	}

		//	if (!dbContext.CompanyPhones.Any())
		//	{
		//		var CompanyPhonesData = File.ReadAllText("../SmartHiring.Repository/Data/DataSeed/CompanyPhones.json");
		//		var CompanyPhones = JsonSerializer.Deserialize<List<CompanyPhone>>(CompanyPhonesData);
		//		if (CompanyPhones?.Count > 0)
		//		{
		//			foreach (var CompanyPhone in CompanyPhones)
		//				await dbContext.Set<CompanyPhone>().AddAsync(CompanyPhone);
		//			await dbContext.SaveChangesAsync();
		//		}
		//	}

		//	if (!dbContext.Applicants.Any())
		//	{
		//		var ApplicantsData = File.ReadAllText("../SmartHiring.Repository/Data/DataSeed/Applicants.json");
		//		var Applicants = JsonSerializer.Deserialize<List<Applicant>>(ApplicantsData);
		//		if (Applicants?.Count > 0)
		//		{
		//			foreach (var Applicant in Applicants)
		//				await dbContext.Set<Applicant>().AddAsync(Applicant);
		//			await dbContext.SaveChangesAsync();
		//		}
		//	}

		//	if (!dbContext.ApplicantAddresses.Any())
		//	{
		//		var ApplicantAddressesData = File.ReadAllText("../SmartHiring.Repository/Data/DataSeed/ApplicantAddresses.json");
		//		var ApplicantAddresses = JsonSerializer.Deserialize<List<ApplicantAddress>>(ApplicantAddressesData);
		//		if (ApplicantAddresses?.Count > 0)
		//		{
		//			foreach (var ApplicantAddress in ApplicantAddresses)
		//				await dbContext.Set<ApplicantAddress>().AddAsync(ApplicantAddress);
		//			await dbContext.SaveChangesAsync();
		//		}
		//	}


		//	if (!dbContext.Applications.Any())
		//	{
		//		var ApplicationsData = File.ReadAllText("../SmartHiring.Repository/Data/DataSeed/Applications.json");
		//		var Applications = JsonSerializer.Deserialize<List<Application>>(ApplicationsData);
		//		if (Applications?.Count > 0)
		//		{
		//			foreach (var Application in Applications)
		//				await dbContext.Set<Application>().AddAsync(Application);
		//			await dbContext.SaveChangesAsync();
		//		}
		//	}

		//	if (!dbContext.Interviews.Any())
		//	{
		//		var InterviewsData = File.ReadAllText("../SmartHiring.Repository/Data/DataSeed/Interviews.json");
		//		var Interviews = JsonSerializer.Deserialize<List<Interview>>(InterviewsData);
		//		if (Interviews?.Count > 0)
		//		{
		//			foreach (var Interview in Interviews)
		//				await dbContext.Set<Interview>().AddAsync(Interview);
		//			await dbContext.SaveChangesAsync();
		//		}
		//	}

		//	if (!dbContext.CandidateLists.Any())
		//	{
		//		var CandidateListsData = File.ReadAllText("../SmartHiring.Repository/Data/DataSeed/CandidateLists.json");
		//		var CandidateLists = JsonSerializer.Deserialize<List<CandidateList>>(CandidateListsData);
		//		if (CandidateLists?.Count > 0)
		//		{
		//			foreach (var CandidateList in CandidateLists)
		//				await dbContext.Set<CandidateList>().AddAsync(CandidateList);
		//			await dbContext.SaveChangesAsync();
		//		}
		//	}

		//	if (!dbContext.CandidateListApplicants.Any())
		//	{
		//		var CandidateListApplicantsData = File.ReadAllText("../SmartHiring.Repository/Data/DataSeed/CandidateListApplicants.json");
		//		var CandidateListApplicants = JsonSerializer.Deserialize<List<CandidateListApplicant>>(CandidateListApplicantsData);
		//		if (CandidateListApplicants?.Count > 0)
		//		{
		//			foreach (var CandidateListApplicant in CandidateListApplicants)
		//				await dbContext.Set<CandidateListApplicant>().AddAsync(CandidateListApplicant);
		//			await dbContext.SaveChangesAsync();
		//		}
		//	}

		//	if (!dbContext.AgencyApplicants.Any())
		//	{
		//		var AgencyApplicantsData = File.ReadAllText("../SmartHiring.Repository/Data/DataSeed/AgencyApplicants.json");
		//		var AgencyApplicants = JsonSerializer.Deserialize<List<AgencyApplicant>>(AgencyApplicantsData);
		//		if (AgencyApplicants?.Count > 0)
		//		{
		//			foreach (var AgencyApplicant in AgencyApplicants)
		//				await dbContext.Set<AgencyApplicant>().AddAsync(AgencyApplicant);
		//			await dbContext.SaveChangesAsync();
		//		}
		//	}
		//	if (!dbContext.ApplicantPhones.Any())
		//	{
		//		var ApplicantPhonesData = File.ReadAllText("../SmartHiring.Repository/Data/DataSeed/ApplicantPhones.json");
		//		var ApplicantPhones = JsonSerializer.Deserialize<List<ApplicantPhone>>(ApplicantPhonesData);
		//		if (ApplicantPhones?.Count > 0)
		//		{
		//			foreach (var ApplicantPhone in ApplicantPhones)
		//				await dbContext.Set<ApplicantPhone>().AddAsync(ApplicantPhone);
		//			await dbContext.SaveChangesAsync();
		//		}
		//	}
		}
	}
}
