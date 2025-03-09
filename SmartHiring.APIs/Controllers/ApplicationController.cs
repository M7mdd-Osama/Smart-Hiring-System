using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartHiring.APIs.DTOs;
using SmartHiring.APIs.Errors;
using SmartHiring.Core.Entities;
using SmartHiring.Core.Entities.Identity;
using SmartHiring.Core.Repositories;
using SmartHiring.Core.Specifications;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartHiring.APIs.Controllers
{

    public class ApplicationController : APIBaseController
    {
        private readonly IGenericRepository<Application> _applicationRepository;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;

        public ApplicationController(
            IGenericRepository<Application> applicationRepository,
            IMapper mapper,
            UserManager<AppUser> userManager)
        {
            _applicationRepository = applicationRepository;
            _mapper = mapper;
            _userManager = userManager;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ApplicationDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ApplicationDto>>> GetApplications()
        {
            var spec = new ApplicationSpecification(0); // جلب كل الطلبات
            var applications = await _applicationRepository.GetAllWithSpecAsync(spec);
            var mappedApplications = _mapper.Map<IEnumerable<Application>, IEnumerable<ApplicationDto>>(applications);
            return Ok(mappedApplications);
        }

        [HttpGet("{applicationId}")]
        [ProducesResponseType(typeof(ApplicationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApplicationDto>> GetApplication(int applicationId)
        {
            var spec = new ApplicationSpecification(applicationId);
            var application = await _applicationRepository.GetByEntityWithSpecAsync(spec);
            if (application == null) return NotFound(new ApiResponse(404));

            var mappedApplication = _mapper.Map<Application, ApplicationDto>(application);
            return Ok(mappedApplication);
        }

        [HttpGet("job/{jobId}")]
        [ProducesResponseType(typeof(IEnumerable<ApplicationDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ApplicationDto>>> GetApplicationsForJob(int jobId)
        {
            var spec = new ApplicationSpecification(jobId);
            var applications = await _applicationRepository.GetAllWithSpecAsync(spec);
            var mappedApplications = _mapper.Map<IEnumerable<Application>, IEnumerable<ApplicationDto>>(applications);
            return Ok(mappedApplications);
        }

        [HttpPost]
        public async Task<ActionResult<ApplicationDto>> CreateApplication([FromBody] ApplicationDto applicationDto)
        {
            if (applicationDto == null || string.IsNullOrEmpty(applicationDto.CV_Link))
                return BadRequest(new { message = "Invalid application data. CV_Link is required." });

            var agency = await _userManager.FindByIdAsync(applicationDto.AgencyId);
            if (agency == null)
                return NotFound(new { message = "Agency not found." });

            var application = new Application
            {
                RankScore = applicationDto.RankScore,
                IsShortlisted = applicationDto.IsShortlisted,
                ApplicationDate = DateTime.UtcNow,
                CV_Link = applicationDto.CV_Link,
                ApplicantId = applicationDto.ApplicantId,
                PostId = applicationDto.PostId,
                AgencyId = applicationDto.AgencyId
            };

            await _applicationRepository.AddAsync(application);
            var result = _mapper.Map<Application, ApplicationDto>(application);
            return Ok(result);
        }

        [HttpPut("{applicationId}/approve")]
        public async Task<IActionResult> ApproveApplication(int applicationId)
        {
            var application = await _applicationRepository.GetByIdAsync(applicationId);
            if (application == null)
                return NotFound(new { message = "Application not found." });

            application.IsShortlisted = true;
            await _applicationRepository.UpdateAsync(application);
            return Ok(new { message = "Application approved successfully", applicationId });
        }

        [HttpPut("{applicationId}/reject")]
        public async Task<IActionResult> RejectApplication(int applicationId)
        {
            var application = await _applicationRepository.GetByIdAsync(applicationId);
            if (application == null)
                return NotFound(new { message = "Application not found." });

            application.IsShortlisted = false;
            await _applicationRepository.UpdateAsync(application);
            return Ok(new { message = "Application rejected successfully", applicationId });
        }

        [HttpDelete("{applicationId}")]
        public async Task<IActionResult> DeleteApplication(int applicationId)
        {
            var application = await _applicationRepository.GetByIdAsync(applicationId);
            if (application == null)
                return NotFound(new { message = "Application not found." });

            await _applicationRepository.DeleteAsync(application);
            return Ok(new { message = "Application deleted successfully" });
        }
    }
}