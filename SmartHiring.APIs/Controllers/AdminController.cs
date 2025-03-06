using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartHiring.APIs.DTOs;
using SmartHiring.Core.Entities;
using SmartHiring.Core.Entities.Identity;
using SmartHiring.Core.Repositories;
using SmartHiring.Core.Specifications;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartHiring.APIs.Controllers
{
    [Route("api/admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IGenericRepository<Company> _companyRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public AdminController(
            IGenericRepository<Company> companyRepository,
            UserManager<AppUser> userManager,
            IMapper mapper)
        {
            _companyRepository = companyRepository;
            _userManager = userManager;
            _mapper = mapper;
        }

        [HttpGet("companies")]
        public async Task<ActionResult<IEnumerable<CompanyDto>>> GetAllCompanies()
        {
            var spec = new CompaniesWithDetailsSpecification();
            var companies = await _companyRepository.GetAllWithSpecAsync(spec);

            if (!companies.Any())
                return NotFound("No companies found.");

            return Ok(_mapper.Map<IEnumerable<CompanyDto>>(companies));
        }

        [HttpDelete("company/{companyId}")]
        public async Task<IActionResult> DeleteCompany(int companyId)
        {
            var company = await _companyRepository.GetByIdAsync(companyId);
            if (company == null)
                return NotFound($"Company with ID {companyId} not found.");

            await _companyRepository.DeleteAsync(company);
            return Ok($"Company with ID {companyId} has been deleted successfully.");
        }

        [HttpPost("company")]
        public async Task<IActionResult> CreateCompany([FromBody] CompanyDto companyDto)
        {
            if (companyDto == null)
                return BadRequest("Invalid company data.");

            var company = _mapper.Map<Company>(companyDto);
            await _companyRepository.AddAsync(company);

            return Ok(_mapper.Map<CompanyDto>(company));
        }

        [HttpPut("company/{companyId}")]
        public async Task<IActionResult> UpdateCompany(int companyId, [FromBody] CompanyDto companyDto)
        {
            var existingCompany = await _companyRepository.GetByIdAsync(companyId);
            if (existingCompany == null)
                return NotFound($"Company with ID {companyId} not found.");

            _mapper.Map(companyDto, existingCompany);
            await _companyRepository.UpdateAsync(existingCompany);

            return Ok(_mapper.Map<CompanyDto>(existingCompany));
        }

        [HttpPost("company/{companyId}/hr")]
        public async Task<IActionResult> CreateHR(int companyId, [FromBody] HRDto hrDto)
        {
            var company = await _companyRepository.GetByIdAsync(companyId);
            if (company == null)
                return NotFound($"Company with ID {companyId} not found.");

            if (company.HR != null)
                return BadRequest($"Company with ID {companyId} already has an HR.");

            var hr = _mapper.Map<AppUser>(hrDto);
            hr.HRCompany = company;

            var result = await _userManager.CreateAsync(hr);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            company.HR = hr;
            await _companyRepository.UpdateAsync(company);

            return Ok(_mapper.Map<HRDto>(hr));
        }

        [HttpGet("company/{companyId}/hr")]
        public async Task<IActionResult> GetHRByCompanyId(int companyId)
        {
            var spec = new CompaniesWithDetailsSpecification(companyId);
            var company = await _companyRepository.GetByIdWithSpecAsync(spec);

            if (company == null || company.HR == null)
                return NotFound($"No HR found for company with ID {companyId}.");

            return Ok(_mapper.Map<HRDto>(company.HR));
        }
    }
}
