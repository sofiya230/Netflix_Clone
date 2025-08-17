using NetflixClone.DTOs;
using NetflixClone.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetflixClone.Interfaces
{
    public interface IWatchHistoryRepository : IBaseRepository<WatchHistory>
    {
        Task<IEnumerable<WatchHistory>> GetWatchHistoryByProfileIdAsync(int profileId);
        Task<WatchHistory?> GetWatchHistoryByContentAndProfileAsync(int contentId, int profileId, int? episodeId = null);
        Task<IEnumerable<WatchHistory>> GetContinueWatchingAsync(int profileId, int limit = 10);
    }
}
