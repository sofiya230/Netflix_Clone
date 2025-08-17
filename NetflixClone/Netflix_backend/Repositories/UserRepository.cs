using Microsoft.EntityFrameworkCore;
using NetflixClone.Models;
using NetflixClone.Interfaces;
using System.Threading.Tasks;
using NetflixClone.Data;

namespace NetflixClone.Infrastructure.Data.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
        }

        public async Task<User?> GetUserWithProfilesAsync(int userId)
        {
            return await _dbContext.Users
                .Include(u => u.Profiles.Where(p => !p.IsDeleted))
                .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);
        }

        public async Task<bool> IsEmailUniqueAsync(string email)
        {
            return !await _dbContext.Users.AnyAsync(u => u.Email == email);
        }
       
       
    }
}
