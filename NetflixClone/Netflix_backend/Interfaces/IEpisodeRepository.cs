using NetflixClone.DTOs;
using NetflixClone.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetflixClone.Interfaces
{
    public interface IEpisodeRepository : IBaseRepository<Episode>
    {
        Task<IEnumerable<Episode>> GetEpisodesByContentIdAsync(int contentId);
        Task<IEnumerable<Episode>> GetEpisodesBySeasonAsync(int contentId, int seasonNumber);
        Task<Episode?> GetEpisodeByContentAndNumberAsync(int contentId, int seasonNumber, int episodeNumber);
    }
}
