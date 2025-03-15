using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartHiring.APIs.DTOs;
using SmartHiring.Core.Entities;
using SmartHiring.Core.Repositories;
using SmartHiring.Core.Specifications;

namespace SmartHiring.APIs.Controllers
{
    public class CompanyController : APIBaseController
    {
        private readonly IGenericRepository<Company> _companyRepo;
        private readonly IMapper _mapper;

        public CompanyController(IGenericRepository<Company> CompanyRepo, IMapper mapper)
        {
            _companyRepo = CompanyRepo;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CompanyToReturnDto>>> GetCompanies()
        {
            var spec = new CompanySpecifications();
            var companies = await _companyRepo.GetAllWithSpecAsync(spec);
            var mappedCompanies = _mapper.Map<IEnumerable<Company>, IEnumerable<CompanyToReturnDto>>(companies);
            return Ok(mappedCompanies);
        }

        [HttpGet("{companyId}")]
        public async Task<ActionResult<CompanyToReturnDto>> GetCompany(int companyId)
        {
            var spec = new CompanySpecifications(companyId);
            var company = await _companyRepo.GetByEntityWithSpecAsync(spec);
            if (company == null) return NotFound();

            var mappedCompany = _mapper.Map<Company, CompanyToReturnDto>(company);
            return Ok(mappedCompany);
        }

        [HttpPost]
        public async Task<ActionResult<CompanyToReturnDto>> CreateCompany([FromBody] CompanyCreateDto companyDto)
        {
            if (companyDto == null) return BadRequest("Invalid company data");

            var company = _mapper.Map<CompanyCreateDto, Company>(companyDto);
            await _companyRepo.AddAsync(company);

            var createdCompany = _mapper.Map<Company, CompanyToReturnDto>(company);
            return CreatedAtAction(nameof(GetCompany), new { companyId = company.Id }, createdCompany);
        }

        [HttpPut("{companyId}")]
        public async Task<IActionResult> UpdateCompany(int companyId, [FromBody] CompanyUpdateDto companyDto)
        {
            var spec = new CompanySpecifications(companyId);
            var company = await _companyRepo.GetByEntityWithSpecAsync(spec);

            if (company == null) return NotFound();

            _mapper.Map(companyDto, company);
            await _companyRepo.UpdateAsync(company);

            return NoContent();
        }

        [HttpDelete("{companyId}")]
        public async Task<IActionResult> DeleteCompany(int companyId)
        {
            var company = await _companyRepo.GetByIdAsync(companyId);
            if (company == null) return NotFound();

            await _companyRepo.DeleteAsync(company);
            return NoContent();
        }
    }
}
