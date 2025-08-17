using Microsoft.EntityFrameworkCore;
using NetflixClone.Models;
using NetflixClone.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NetflixClone.Data;

namespace NetflixClone.Infrastructure.Data.Repositories
{
    public class EpisodeRepository : BaseRepository<Episode>, IEpisodeRepository
    {
        public EpisodeRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<Episode>> GetEpisodesByContentIdAsync(int contentId)
        {
            return await _dbContext.Episodes
                .Where(e => e.ContentId == contentId && !e.IsDeleted)
                .OrderBy(e => e.SeasonNumber)
                .ThenBy(e => e.EpisodeNumber)
                .ToListAsync();
        }

        public async Task<IEnumerable<Episode>> GetEpisodesBySeasonAsync(int contentId, int seasonNumber)
        {
            return await _dbContext.Episodes
                .Where(e => e.ContentId == contentId && 
                            e.SeasonNumber == seasonNumber && 
                            !e.IsDeleted)
                .OrderBy(e => e.EpisodeNumber)
                .ToListAsync();
        }

        public async Task<Episode?> GetEpisodeByContentAndNumberAsync(int contentId, int seasonNumber, int episodeNumber)
        {
            return await _dbContext.Episodes
                .FirstOrDefaultAsync(e => e.ContentId == contentId &&
                                        e.SeasonNumber == seasonNumber &&
                                        e.EpisodeNumber == episodeNumber &&
                                        !e.IsDeleted);
        }
    }
}
