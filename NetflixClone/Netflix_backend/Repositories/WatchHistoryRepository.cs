using Microsoft.EntityFrameworkCore;
using NetflixClone.Models;
using NetflixClone.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NetflixClone.Data;

namespace NetflixClone.Infrastructure.Data.Repositories
{
    public class WatchHistoryRepository : BaseRepository<WatchHistory>, IWatchHistoryRepository
    {
        public WatchHistoryRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<WatchHistory>> GetWatchHistoryByProfileIdAsync(int profileId)
        {
            var watchHistories = await _dbContext.WatchHistories
                .Where(w => w.ProfileId == profileId && !w.IsDeleted)
                .Include(w => w.Content)
                .Include(w => w.Episode)
                .OrderByDescending(w => w.LastWatched)
                .ToListAsync();

            var groupedHistories = watchHistories
                .GroupBy(w => w.ContentId)
                .Select(g => g.OrderByDescending(w => w.LastWatched).First())
                .OrderByDescending(w => w.LastWatched)
                .ToList();

            return groupedHistories;
        }

        public async Task<WatchHistory?> GetWatchHistoryByContentAndProfileAsync(int contentId, int profileId, int? episodeId = null)
        {
            return await _dbContext.WatchHistories
                .FirstOrDefaultAsync(w => w.ContentId == contentId &&
                                         w.ProfileId == profileId &&
                                         (episodeId == null || w.EpisodeId == episodeId) &&
                                         !w.IsDeleted);
        }

        public async Task<IEnumerable<WatchHistory>> GetContinueWatchingAsync(int profileId, int limit = 10)
        {
            var watchHistories = await _dbContext.WatchHistories
                .Where(w => w.ProfileId == profileId && 
                            !w.IsCompleted && 
                            !w.IsDeleted)
                .Include(w => w.Content)
                .Include(w => w.Episode)
                .OrderByDescending(w => w.LastWatched)
                .ToListAsync();

            var groupedHistories = watchHistories
                .GroupBy(w => w.ContentId)
                .Select(g => g.OrderByDescending(w => w.LastWatched).First())
                .OrderByDescending(w => w.LastWatched)
                .Take(limit)
                .ToList();

            return groupedHistories;
        }
    }
}
