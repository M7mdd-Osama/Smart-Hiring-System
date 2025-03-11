using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using SmartHiring.APIs.DTOs;
using SmartHiring.Core.Entities;
using SmartHiring.Core.Repositories;
using SmartHiring.Core.Specifications;

namespace SmartHiring.APIs.Controllers
{
    public class UserController : APIsBaseController
    {
        private readonly IGenericRepository<Company> _userRepo;
        private readonly IMapper _mapper;

        public UserController(IGenericRepository<Company> UserRepo , IMapper mapper)
        {
            _userRepo = UserRepo;
            _mapper = mapper;
        }
        //------------------------------------------------------------------------------------------
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Company>>> GetUsers() 
        {
            var spec = new CompanyWithHrandAdminSpacifications();
            var Users = await _userRepo.GetAllWithSpecAsync(spec);
            var MappedPosts = _mapper.Map<IEnumerable<Company>, IEnumerable<UserToReturnDto>>(Users);
            return Ok(MappedPosts);
        }
        //------------------------------------------------------------------------------------------
        [HttpGet("{id}")]
        public async Task<ActionResult<Company>> GetUser(int id)
        {
            var spec = new CompanyWithHrandAdminSpacifications(id);
            var user = await _userRepo.GetByIdWithSpecAsync(spec);
            var MappedPosts = _mapper.Map<Company, UserToReturnDto>(user);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(MappedPosts);
        }
        //------------------------------------------------------------------------------------------
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUser(int id, [FromBody] UserUpdateDto userUpdateDto)
        {
            var spec = new CompanyWithHrandAdminSpacifications(id);
            var user = await _userRepo.GetByIdWithSpecAsync(spec);

            if (user == null)
            {
                return NotFound();
            }

            user.Name = userUpdateDto.Name ?? user.Name;
            user.Location = userUpdateDto.Location ?? user.Location;
            user.Industry = userUpdateDto.Industry ?? user.Industry;
            user.BusinessEmail = userUpdateDto.BusinessEmail ?? user.BusinessEmail;

            await _userRepo.UpdateAsync(user);
            return NoContent();
        }
        //------------------------------------------------------------------------------------------
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            var user = await _userRepo.GetByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            await _userRepo.DeleteAsync(user);
            return NoContent();
        }

    }
}
