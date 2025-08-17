using NetflixClone.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetflixClone.Interfaces
{
    public interface IEpisodeService
    {
        Task<EpisodeResponseDto> GetEpisodeByIdAsync(int episodeId);
        Task<IEnumerable<EpisodeResponseDto>> GetEpisodesByContentIdAsync(int contentId);
        Task<IEnumerable<EpisodeResponseDto>> GetEpisodesBySeasonAsync(int contentId, int seasonNumber);
        Task<EpisodeResponseDto> GetEpisodeByContentAndNumberAsync(int contentId, int seasonNumber, int episodeNumber);
        Task<EpisodeResponseDto> CreateEpisodeAsync(CreateEpisodeDto createDto);
        Task<EpisodeResponseDto> UpdateEpisodeAsync(int episodeId, UpdateEpisodeDto updateDto);
        Task<bool> DeleteEpisodeAsync(int episodeId);
    }
}
