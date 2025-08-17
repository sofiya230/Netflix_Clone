using NetflixClone.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetflixClone.Interfaces
{
    public interface IContentService
    {
        Task<ContentResponseDto> GetContentByIdAsync(int contentId);
        Task<IEnumerable<ContentSummaryDto>> GetAllContentsAsync();
        Task<IEnumerable<ContentSummaryDto>> GetContentsByGenreAsync(string genre);
        Task<IEnumerable<ContentSummaryDto>> GetContentsByTypeAsync(string contentType);
        Task<ContentResponseDto> CreateContentAsync(CreateContentDto createDto);
        Task<ContentResponseDto> UpdateContentAsync(int contentId, UpdateContentDto updateDto);
        Task<bool> DeleteContentAsync(int contentId);
        Task<IEnumerable<ContentSummaryDto>> GetTrendingContentAsync(int limit = 10);
        Task<IEnumerable<ContentSummaryDto>> GetNewReleasesAsync(int limit = 10);
        Task<IEnumerable<ContentSummaryDto>> SearchContentsAsync(string searchTerm);
    }
}
