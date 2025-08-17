using Microsoft.EntityFrameworkCore;
using NetflixClone.Models;
using NetflixClone.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NetflixClone.Data;

namespace NetflixClone.Infrastructure.Data.Repositories
{
    public class ProfileRepository : BaseRepository<Profile>, IProfileRepository
    {
        public ProfileRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<Profile>> GetProfilesByUserIdAsync(int userId)
        {
            return await _dbContext.Profiles
                .Where(p => p.UserId == userId && !p.IsDeleted)
                .ToListAsync();
        }

        public async Task<int> GetProfileCountByUserIdAsync(int userId)
        {
            return await _dbContext.Profiles
                .CountAsync(p => p.UserId == userId && !p.IsDeleted);
        }
    }
}
