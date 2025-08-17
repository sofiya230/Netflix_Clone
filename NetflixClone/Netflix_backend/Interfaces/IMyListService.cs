using NetflixClone.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetflixClone.Interfaces
{
    public interface IMyListService
    {
        Task<IEnumerable<MyListResponseDto>> GetMyListByProfileIdAsync(int profileId);
        Task<MyListResponseDto> AddToMyListAsync(MyListCreateDto createDto);
        Task<bool> RemoveFromMyListAsync(int contentId, int profileId);
        Task<bool> IsContentInMyListAsync(int contentId, int profileId);
    }
}
