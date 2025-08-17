using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetflixClone.DTOs;
using NetflixClone.Interfaces;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NetflixClone.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfilesController : ControllerBase
    {
        private readonly IProfileService _profileService;
        private readonly IUserService _userService;

        public ProfilesController(IProfileService profileService, IUserService userService)
        {
            _profileService = profileService;
            _userService = userService;
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<ProfileResponseDto>>> GetProfilesByUserId(int userId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId != userId.ToString() && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            try
            {
                var profiles = await _profileService.GetProfilesByUserIdAsync(userId);
                return Ok(profiles);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProfileResponseDto>> GetProfile(int id)
        {
            try
            {
                var profile = await _profileService.GetProfileByIdAsync(id);
                
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (currentUserId != profile.UserId.ToString() && !User.IsInRole("Admin"))
                {
                    return Forbid();
                }
                
                return Ok(profile);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("user/{userId}")]
        public async Task<ActionResult<ProfileResponseDto>> CreateProfile(int userId, ProfileCreateDto createDto)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId != userId.ToString() && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            try
            {
                var profile = await _profileService.CreateProfileAsync(userId, createDto);
                return CreatedAtAction(nameof(GetProfile), new { id = profile.Id }, profile);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ProfileResponseDto>> UpdateProfile(int id, ProfileUpdateDto updateDto)
        {
            try
            {
                var profile = await _profileService.GetProfileByIdAsync(id);
                
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (currentUserId != profile.UserId.ToString() && !User.IsInRole("Admin"))
                {
                    return Forbid();
                }
                
                var updatedProfile = await _profileService.UpdateProfileAsync(id, updateDto);
                return Ok(updatedProfile);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProfile(int id)
        {
            try
            {
                var profile = await _profileService.GetProfileByIdAsync(id);
                
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (currentUserId != profile.UserId.ToString() && !User.IsInRole("Admin"))
                {
                    return Forbid();
                }
                
                var result = await _profileService.DeleteProfileAsync(id);
                if (result)
                {
                    return Ok(new { message = "Profile deleted successfully" });
                }
                return BadRequest(new { message = "Failed to delete profile" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
