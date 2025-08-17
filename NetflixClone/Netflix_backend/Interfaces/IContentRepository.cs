using NetflixClone.DTOs;
using NetflixClone.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetflixClone.Interfaces
{
    public interface IContentRepository : IBaseRepository<Content>
    {
        Task<IEnumerable<Content>> GetContentByGenreAsync(string genre);
        Task<IEnumerable<Content>> GetContentByTypeAsync(string contentType);
        Task<Content?> GetContentWithEpisodesAsync(int contentId);
        Task<IEnumerable<Content>> GetTrendingContentAsync(int limit = 10);
        Task<IEnumerable<Content>> GetNewReleasesAsync(int limit = 10);
    }
}
