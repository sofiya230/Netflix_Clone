using Microsoft.EntityFrameworkCore;
using NetflixClone.Models;
using NetflixClone.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NetflixClone.Data;

namespace NetflixClone.Infrastructure.Data.Repositories
{
    public class ContentRepository : BaseRepository<Content>, IContentRepository
    {
        public ContentRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<Content>> GetContentByGenreAsync(string genre)
        {
            return await _dbContext.Contents
                .Where(c => c.Genre.Contains(genre) && !c.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<Content>> GetContentByTypeAsync(string contentType)
        {
            return await _dbContext.Contents
                .Where(c => c.ContentType == contentType && !c.IsDeleted)
                .ToListAsync();
        }

        public async Task<Content?> GetContentWithEpisodesAsync(int contentId)
        {
            return await _dbContext.Contents
                .Include(c => c.Episodes.Where(e => !e.IsDeleted))
                .FirstOrDefaultAsync(c => c.Id == contentId && !c.IsDeleted);
        }

        public async Task<IEnumerable<Content>> GetTrendingContentAsync(int limit = 10)
        {
            var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
            
            var trendingContentIds = await _dbContext.WatchHistories
                .Where(w => w.LastWatched >= thirtyDaysAgo)
                .GroupBy(w => w.ContentId)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .Take(limit)
                .ToListAsync();

            if (!trendingContentIds.Any())
            {
                            return await _dbContext.Contents
                    .Where(c => !c.IsDeleted)
                    .OrderByDescending(c => c.ReleaseYear)
                    .Take(limit)
                    .ToListAsync();
            }

            return await _dbContext.Contents
                .Where(c => trendingContentIds.Contains(c.Id) && !c.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<Content>> GetNewReleasesAsync(int limit = 10)
        {
            return await _dbContext.Contents
                .Where(c => !c.IsDeleted)
                .OrderByDescending(c => c.ReleaseYear)
                .ThenByDescending(c => c.CreatedAt)
                .Take(limit)
                .ToListAsync();
        }
    }
}
