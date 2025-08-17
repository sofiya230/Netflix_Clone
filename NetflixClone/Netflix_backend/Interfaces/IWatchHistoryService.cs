using NetflixClone.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetflixClone.Interfaces
{
    public interface IWatchHistoryService
    {
        Task<IEnumerable<WatchHistoryResponseDto>> GetWatchHistoryByProfileIdAsync(int profileId);
        Task<WatchHistoryResponseDto> GetWatchHistoryByIdAsync(int watchHistoryId);
        Task<WatchHistoryResponseDto> GetWatchHistoryByContentAndProfileAsync(int contentId, int profileId, int? episodeId = null);
        Task<WatchHistoryResponseDto> CreateOrUpdateWatchHistoryAsync(WatchHistoryCreateDto createDto);
        Task<WatchHistoryResponseDto> UpdateWatchHistoryAsync(int watchHistoryId, WatchHistoryUpdateDto updateDto);
        Task<bool> DeleteWatchHistoryAsync(int watchHistoryId);
        Task<IEnumerable<ContinueWatchingDto>> GetContinueWatchingAsync(int profileId, int limit = 10);
        Task<IEnumerable<WatchHistoryResponseDto>> GetCompletedWatchHistoryAsync(int profileId);
    }
}
