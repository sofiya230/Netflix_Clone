using NetflixClone.DTOs;
using NetflixClone.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetflixClone.Interfaces
{
    public interface IProfileRepository : IBaseRepository<Profile>
    {
        Task<IEnumerable<Profile>> GetProfilesByUserIdAsync(int userId);
        Task<int> GetProfileCountByUserIdAsync(int userId);
    }
}
