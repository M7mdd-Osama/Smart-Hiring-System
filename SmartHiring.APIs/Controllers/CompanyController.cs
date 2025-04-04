﻿using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartHiring.APIs.DTOs;
using SmartHiring.APIs.Errors;
using SmartHiring.Core.Entities;
using SmartHiring.Core.Entities.Identity;
using SmartHiring.Core.Repositories;
using SmartHiring.Core.Specifications;

namespace SmartHiring.APIs.Controllers
{
    public class CompanyController : APIBaseController
    {
        private readonly IGenericRepository<Company> _companyRepo;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        private readonly IGenericRepository<Note> _noteRepo;
        private readonly IGenericRepository<Post> _postRepo;

        public CompanyController(IGenericRepository<Company> companyRepo,
            IMapper mapper,
            UserManager<AppUser> userManager,
            IGenericRepository<Note> noteRepo,
            IGenericRepository<Post> postRepo)
        {
            _companyRepo = companyRepo;
            _mapper = mapper;
            _userManager = userManager;
            _noteRepo = noteRepo;
            _postRepo = postRepo;
        }

        #region Get Company Members

        [Authorize(Roles = "HR")]
        [HttpGet("members")]
        public async Task<IActionResult> GetCompanyMembers()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized(new ApiResponse(401, "User email not found in token"));

            var user = await _userManager.Users
                .Include(u => u.HRCompany)
                .FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
                return Unauthorized(new ApiResponse(401, "User not found"));

            if (user.HRCompany == null)
                return BadRequest(new ApiResponse(400, "User is not assigned to any company"));

            var company = await _companyRepo.GetByEntityWithSpecAsync(
                new CompanyWithMembersSpecifications(user.HRCompany.Id)
            );

            if (company == null)
                return NotFound(new ApiResponse(404, "Company not found"));

            var result = _mapper.Map<CompanyMembersDto>(company);
            return Ok(result);
        }

        #endregion

        #region Create New Note

        [Authorize(Roles = "HR,Manager")]
        [HttpPost("notes")]
        public async Task<IActionResult> CreateNote([FromBody] CreateNoteDto noteDto)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized(new ApiResponse(401, "User email not found in token"));

            var user = await _userManager.Users
                .Include(u => u.HRCompany)
                .Include(u => u.ManagedCompany)
                .FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
                return Unauthorized(new ApiResponse(401, "User not found"));

            var companyId = user.HRCompany?.Id ?? user.ManagedCompany?.Id;

            if (companyId == null)
                return Unauthorized(new ApiResponse(401, "User is not associated with any company"));

            var post = await _postRepo.GetByIdAsync(noteDto.PostId);
            if (post == null)
                return BadRequest(new ApiResponse(400, "Post not found"));

            var note = _mapper.Map<Note>(noteDto);
            note.UserId = user.Id;
            note.CreatedAt = DateTime.Now;

            await _noteRepo.AddAsync(note);
            return Ok(new ApiResponse(200, "Note created successfully"));
        }

        #endregion

        #region Get All Notes

        [Authorize(Roles = "HR,Manager")]
        [HttpGet("notes")]
        public async Task<IActionResult> GetNotesByCompany([FromQuery] int? postId)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized(new ApiResponse(401, "User email not found in token"));

            var user = await _userManager.Users
                .Include(u => u.HRCompany)
                .Include(u => u.ManagedCompany)
                .FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
                return Unauthorized(new ApiResponse(401, "User not found"));

            var companyId = user.HRCompany?.Id ?? user.ManagedCompany?.Id;

            if (companyId == null)
                return Unauthorized(new ApiResponse(401, "User is not associated with any company"));

            var spec = new NotesByCompanySpec(companyId.Value, postId);
            var notes = await _noteRepo.GetAllWithSpecAsync(spec);

            if (notes == null || !notes.Any())
                return NotFound(new ApiResponse(404, "No notes found for this company"));

            var roles = await _userManager.GetRolesAsync(user);

            var result = _mapper.Map<List<NoteDto>>(notes, opt =>
            {
                opt.Items["UserEmail"] = userEmail;
                opt.Items["UserRoles"] = roles;
            });

            return Ok(result);
        }

        #endregion

        #region Get Note By Id

        [HttpGet("notes/{noteId}")]
        public async Task<IActionResult> GetNoteById(int noteId)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized(new ApiResponse(401, "User email not found in token"));

            var user = await _userManager.Users
                .Include(u => u.HRCompany)
                .Include(u => u.ManagedCompany)
                .FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
                return Unauthorized(new ApiResponse(401, "User not found"));

            var companyId = user.HRCompany?.Id ?? user.ManagedCompany?.Id;

            if (companyId == null)
                return Unauthorized(new ApiResponse(401, "User is not associated with any company"));

            var spec = new NoteByIdAndCompanySpec(noteId, companyId.Value);
            var note = await _noteRepo.GetByEntityWithSpecAsync(spec);

            if (note == null)
                return NotFound(new ApiResponse(404, "Note not found or does not belong to your company"));

            if (note.UserId != user.Id && !note.IsSeen)
            {
                note.IsSeen = true;
                await _noteRepo.SaveChangesAsync();
            }

            var roles = await _userManager.GetRolesAsync(user);

            var noteDto = _mapper.Map<NoteDto>(note, opt =>
            {
                opt.Items["UserEmail"] = userEmail;
                opt.Items["UserRoles"] = roles;
            });

            return Ok(noteDto);
        }

        #endregion

        #region Delete Note

        [Authorize(Roles = "HR,Manager")]
        [HttpDelete("notes/{noteId}")]
        public async Task<IActionResult> DeleteNote(int noteId)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized(new ApiResponse(401, "User email not found in token"));

            var user = await _userManager.Users
                .Include(u => u.HRCompany)
                .Include(u => u.ManagedCompany)
                .FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
                return Unauthorized(new ApiResponse(401, "User not found"));

            var companyId = user.HRCompany?.Id ?? user.ManagedCompany?.Id;

            var spec = new NoteByIdSpecification(noteId, user.Id);
            var notes = await _noteRepo.GetAllWithSpecAsync(spec);

            var note = notes.FirstOrDefault();
            if (note == null)
                return NotFound(new ApiResponse(404, "Note not found"));

            if (note.UserId != user.Id)
            {
                return Unauthorized(new ApiResponse(403, "You can only delete your own notes"));
            }

            await _noteRepo.DeleteAsync(note);
            return Ok(new ApiResponse(200, "Note deleted successfully"));
        }

        #endregion

    }
}
