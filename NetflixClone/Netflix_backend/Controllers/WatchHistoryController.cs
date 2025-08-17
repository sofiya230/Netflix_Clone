using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetflixClone.DTOs;
using NetflixClone.Interfaces;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Linq;
using System;

namespace NetflixClone.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WatchHistoryController : ControllerBase
    {
        private readonly IWatchHistoryService _watchHistoryService;
        private readonly IProfileService _profileService;
        private readonly ILogger<WatchHistoryController> _logger;

        public WatchHistoryController(IWatchHistoryService watchHistoryService, IProfileService profileService, ILogger<WatchHistoryController> logger)
        {
            _watchHistoryService = watchHistoryService;
            _profileService = profileService;
            _logger = logger;
        }

        [HttpGet("profile/{profileId}")]
        public async Task<ActionResult<IEnumerable<WatchHistoryResponseDto>>> GetWatchHistoryByProfileId(int profileId)
        {
            try
            {
                var profile = await _profileService.GetProfileByIdAsync(profileId);
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId != profile.UserId.ToString() && !User.IsInRole("Admin"))
                {
                    return Forbid();
                }

                var watchHistory = await _watchHistoryService.GetWatchHistoryByProfileIdAsync(profileId);
                
                return Ok(watchHistory);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WatchHistoryResponseDto>> GetWatchHistory(int id)
        {
            try
            {
                var watchHistory = await _watchHistoryService.GetWatchHistoryByIdAsync(id);
                
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId != watchHistory.UserId.ToString() && !User.IsInRole("Admin"))
                {
                    return Forbid();
                }
                
                return Ok(watchHistory);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("content/{contentId}/profile/{profileId}")]
        public async Task<ActionResult<WatchHistoryResponseDto>> GetWatchHistoryByContentAndProfile(
            int contentId, int profileId, [FromQuery] int? episodeId = null)
        {
            try
            {
                var profile = await _profileService.GetProfileByIdAsync(profileId);
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId != profile.UserId.ToString() && !User.IsInRole("Admin"))
                {
                    return Forbid();
                }

                var watchHistory = await _watchHistoryService.GetWatchHistoryByContentAndProfileAsync(contentId, profileId, episodeId);
                return Ok(watchHistory);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("continue-watching/profile/{profileId}")]
        public async Task<ActionResult<IEnumerable<ContinueWatchingDto>>> GetContinueWatching(
            int profileId, [FromQuery] int limit = 10)
        {
            try
            {
                var profile = await _profileService.GetProfileByIdAsync(profileId);
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId != profile.UserId.ToString() && !User.IsInRole("Admin"))
                {
                    return Forbid();
                }

                var continueWatching = await _watchHistoryService.GetContinueWatchingAsync(profileId, limit);
                return Ok(continueWatching);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("completed/profile/{profileId}")]
        public async Task<ActionResult<IEnumerable<WatchHistoryResponseDto>>> GetCompletedWatchHistory(int profileId)
        {
            try
            {
                var profile = await _profileService.GetProfileByIdAsync(profileId);
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId != profile.UserId.ToString() && !User.IsInRole("Admin"))
                {
                    return Forbid();
                }

                var completedHistory = await _watchHistoryService.GetCompletedWatchHistoryAsync(profileId);
                return Ok(completedHistory);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<WatchHistoryResponseDto>> CreateOrUpdateWatchHistory(WatchHistoryCreateDto createDto)
        {
            try
            {
                var profile = await _profileService.GetProfileByIdAsync(createDto.ProfileId);
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId != profile.UserId.ToString() && !User.IsInRole("Admin"))
                {
                    return Forbid();
                }

                var watchHistory = await _watchHistoryService.CreateOrUpdateWatchHistoryAsync(createDto);
                return Ok(watchHistory);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<WatchHistoryResponseDto>> UpdateWatchHistory(int id, WatchHistoryUpdateDto updateDto)
        {
            try
            {
                var existingWatchHistory = await _watchHistoryService.GetWatchHistoryByIdAsync(id);
                
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId != existingWatchHistory.UserId.ToString() && !User.IsInRole("Admin"))
                {
                    return Forbid();
                }
                
                var watchHistory = await _watchHistoryService.UpdateWatchHistoryAsync(id, updateDto);
                return Ok(watchHistory);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteWatchHistory(int id)
        {
            try
            {
                var existingWatchHistory = await _watchHistoryService.GetWatchHistoryByIdAsync(id);
                
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId != existingWatchHistory.UserId.ToString() && !User.IsInRole("Admin"))
                {
                    return Forbid();
                }
                
                var result = await _watchHistoryService.DeleteWatchHistoryAsync(id);
                if (result)
                {
                    return Ok(new { message = "Watch history deleted successfully" });
                }
                return BadRequest(new { message = "Failed to delete watch history" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }


    }
}
