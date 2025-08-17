using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetflixClone.DTOs;
using NetflixClone.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace NetflixClone.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContentsController : ControllerBase
    {
        private readonly IContentService _contentService;

        public ContentsController(IContentService contentService)
        {
            _contentService = contentService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContentSummaryDto>>> GetAllContents()
        {
            var contents = await _contentService.GetAllContentsAsync();
            return Ok(contents);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ContentResponseDto>> GetContent(int id)
        {
            try
            {
                var content = await _contentService.GetContentByIdAsync(id);
                return Ok(content);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("genre/{genre}")]
        public async Task<ActionResult<IEnumerable<ContentSummaryDto>>> GetContentsByGenre(string genre)
        {
            var contents = await _contentService.GetContentsByGenreAsync(genre);
            return Ok(contents);
        }

        [HttpGet("type/{contentType}")]
        public async Task<ActionResult<IEnumerable<ContentSummaryDto>>> GetContentsByType(string contentType)
        {
            var contents = await _contentService.GetContentsByTypeAsync(contentType);
            return Ok(contents);
        }

        [HttpGet("trending")]
        public async Task<ActionResult<IEnumerable<ContentSummaryDto>>> GetTrendingContents([FromQuery] int limit = 10)
        {
            var contents = await _contentService.GetTrendingContentAsync(limit);
            return Ok(contents);
        }

        [HttpGet("new-releases")]
        public async Task<ActionResult<IEnumerable<ContentSummaryDto>>> GetNewReleases([FromQuery] int limit = 10)
        {
            var contents = await _contentService.GetNewReleasesAsync(limit);
            return Ok(contents);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<ContentSummaryDto>>> SearchContents([FromQuery] string query)
        {
            var contents = await _contentService.SearchContentsAsync(query);
            return Ok(contents);
        }

        [HttpGet("{id}/similar")]
        public async Task<ActionResult<IEnumerable<ContentSummaryDto>>> GetSimilarContent(int id)
        {
            try
            {
                var content = await _contentService.GetContentByIdAsync(id);
                if (content == null)
                {
                    return NotFound("Content not found");
                }

                var similarContents = await _contentService.GetContentsByGenreAsync(content.Genre);
                var filtered = similarContents.Where(c => c.Id != id).Take(10);
                return Ok(filtered);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ContentResponseDto>> CreateContent(CreateContentDto createDto)
        {
            try
            {
                var content = await _contentService.CreateContentAsync(createDto);
                return CreatedAtAction(nameof(GetContent), new { id = content.Id }, content);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<ContentResponseDto>> UpdateContent(int id, UpdateContentDto updateDto)
        {
            try
            {
                var content = await _contentService.UpdateContentAsync(id, updateDto);
                return Ok(content);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteContent(int id)
        {
            try
            {
                var result = await _contentService.DeleteContentAsync(id);
                if (result)
                {
                    return Ok(new { message = "Content deleted successfully" });
                }
                return BadRequest(new { message = "Failed to delete content" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
