using Microsoft.EntityFrameworkCore;
using NetflixClone.Models;
using NetflixClone.Interfaces;
using NetflixClone.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetflixClone.Infrastructure.Data.Repositories
{
    public class MyListRepository : BaseRepository<MyList>, IMyListRepository
    {
        public MyListRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<MyList>> GetMyListByProfileIdAsync(int profileId)
        {
            return await _dbContext.MyLists
                .Where(m => m.ProfileId == profileId && !m.IsDeleted)
                .Include(m => m.Content)
                .OrderByDescending(m => m.AddedAt)
                .ToListAsync();
        }

        public async Task<bool> IsContentInMyListAsync(int contentId, int profileId)
        {
            return await _dbContext.MyLists
                .AnyAsync(m => m.ContentId == contentId && 
                               m.ProfileId == profileId && 
                               !m.IsDeleted);
        }

        public async Task<bool> RemoveFromMyListAsync(int contentId, int profileId)
        {
            var myListItem = await _dbContext.MyLists
                .FirstOrDefaultAsync(m => m.ContentId == contentId &&
                                          m.ProfileId == profileId &&
                                          !m.IsDeleted);

            if (myListItem == null)
            {
                return false;
            }

            return await SoftDeleteAsync(myListItem.Id);
        }

        public async Task<MyList?> GetMyListItemAsync(int contentId, int profileId)
        {
            return await _dbContext.MyLists
                .Where(m => m.ContentId == contentId && 
                           m.ProfileId == profileId && 
                           !m.IsDeleted)
                .Include(m => m.Content)
                .FirstOrDefaultAsync();
        }
    }
}
