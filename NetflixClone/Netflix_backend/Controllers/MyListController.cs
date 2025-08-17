using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetflixClone.DTOs;
using NetflixClone.Interfaces;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NetflixClone.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MyListController : ControllerBase
    {
        private readonly IMyListService _myListService;
        private readonly IProfileService _profileService;

        public MyListController(IMyListService myListService, IProfileService profileService)
        {
            _myListService = myListService;
            _profileService = profileService;
        }

        [HttpGet("profile/{profileId}")]
        public async Task<ActionResult<IEnumerable<MyListResponseDto>>> GetMyListByProfileId(int profileId)
        {
            try
            {
                var profile = await _profileService.GetProfileByIdAsync(profileId);
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId != profile.UserId.ToString() && !User.IsInRole("Admin"))
                {
                    return Forbid();
                }

                var myList = await _myListService.GetMyListByProfileIdAsync(profileId);
                return Ok(myList);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<MyListResponseDto>> AddToMyList(MyListCreateDto createDto)
        {
            try
            {
                var profile = await _profileService.GetProfileByIdAsync(createDto.ProfileId);
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId != profile.UserId.ToString() && !User.IsInRole("Admin"))
                {
                    return Forbid();
                }

                var myListItem = await _myListService.AddToMyListAsync(createDto);
                return Ok(myListItem);
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

        [HttpDelete("content/{contentId}/profile/{profileId}")]
        public async Task<ActionResult> RemoveFromMyList(int contentId, int profileId)
        {
            try
            {
                var profile = await _profileService.GetProfileByIdAsync(profileId);
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId != profile.UserId.ToString() && !User.IsInRole("Admin"))
                {
                    return Forbid();
                }

                var result = await _myListService.RemoveFromMyListAsync(contentId, profileId);
                if (result)
                {
                    return Ok(new { message = "Content removed from My List successfully" });
                }
                return NotFound(new { message = "Content not found in My List" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("check/content/{contentId}/profile/{profileId}")]
        public async Task<ActionResult<bool>> IsContentInMyList(int contentId, int profileId)
        {
            try
            {
                var profile = await _profileService.GetProfileByIdAsync(profileId);
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId != profile.UserId.ToString() && !User.IsInRole("Admin"))
                {
                    return Forbid();
                }

                var isInMyList = await _myListService.IsContentInMyListAsync(contentId, profileId);
                return Ok(isInMyList);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
