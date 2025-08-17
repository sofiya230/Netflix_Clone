using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetflixClone.DTOs;
using NetflixClone.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetflixClone.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EpisodesController : ControllerBase
    {
        private readonly IEpisodeService _episodeService;

        public EpisodesController(IEpisodeService episodeService)
        {
            _episodeService = episodeService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EpisodeResponseDto>> GetEpisode(int id)
        {
            try
            {
                var episode = await _episodeService.GetEpisodeByIdAsync(id);
                return Ok(episode);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("content/{contentId}")]
        public async Task<ActionResult<IEnumerable<EpisodeResponseDto>>> GetEpisodesByContentId(int contentId)
        {
            var episodes = await _episodeService.GetEpisodesByContentIdAsync(contentId);
            return Ok(episodes);
        }

        [HttpGet("content/{contentId}/season/{seasonNumber}")]
        public async Task<ActionResult<IEnumerable<EpisodeResponseDto>>> GetEpisodesBySeason(int contentId, int seasonNumber)
        {
            var episodes = await _episodeService.GetEpisodesBySeasonAsync(contentId, seasonNumber);
            return Ok(episodes);
        }

        [HttpGet("content/{contentId}/season/{seasonNumber}/episode/{episodeNumber}")]
        public async Task<ActionResult<EpisodeResponseDto>> GetEpisodeByContentAndNumber(int contentId, int seasonNumber, int episodeNumber)
        {
            try
            {
                var episode = await _episodeService.GetEpisodeByContentAndNumberAsync(contentId, seasonNumber, episodeNumber);
                return Ok(episode);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<EpisodeResponseDto>> CreateEpisode(CreateEpisodeDto createDto)
        {
            try
            {
                var episode = await _episodeService.CreateEpisodeAsync(createDto);
                return CreatedAtAction(nameof(GetEpisode), new { id = episode.Id }, episode);
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
        [Authorize]
        public async Task<ActionResult<EpisodeResponseDto>> UpdateEpisode(int id, UpdateEpisodeDto updateDto)
        {
            try
            {
                var episode = await _episodeService.UpdateEpisodeAsync(id, updateDto);
                return Ok(episode);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteEpisode(int id)
        {
            try
            {
                var result = await _episodeService.DeleteEpisodeAsync(id);
                if (result)
                {
                    return Ok(new { message = "Episode deleted successfully" });
                }
                return BadRequest(new { message = "Failed to delete episode" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
