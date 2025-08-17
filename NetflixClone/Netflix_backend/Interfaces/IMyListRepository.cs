using NetflixClone.DTOs;
using NetflixClone.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetflixClone.Interfaces
{
    public interface IMyListRepository : IBaseRepository<MyList>
    {
        Task<IEnumerable<MyList>> GetMyListByProfileIdAsync(int profileId);
        Task<bool> IsContentInMyListAsync(int contentId, int profileId);
        Task<bool> RemoveFromMyListAsync(int contentId, int profileId);
        Task<MyList?> GetMyListItemAsync(int contentId, int profileId);
    }
}
