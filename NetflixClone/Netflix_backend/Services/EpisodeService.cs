using NetflixClone.DTOs;
using NetflixClone.Models;
using NetflixClone.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetflixClone.Services
{
    public class EpisodeService : IEpisodeService
    {
        private readonly IEpisodeRepository _episodeRepository;
        private readonly IContentRepository _contentRepository;

        public EpisodeService(IEpisodeRepository episodeRepository, IContentRepository contentRepository)
        {
            _episodeRepository = episodeRepository;
            _contentRepository = contentRepository;
        }

        public async Task<EpisodeResponseDto> GetEpisodeByIdAsync(int episodeId)
        {
            var episode = await _episodeRepository.GetByIdAsync(episodeId);
            if (episode == null)
            {
                throw new KeyNotFoundException($"Episode with ID {episodeId} not found");
            }

            return MapEpisodeToEpisodeResponseDto(episode);
        }

        public async Task<IEnumerable<EpisodeResponseDto>> GetEpisodesByContentIdAsync(int contentId)
        {
            var episodes = await _episodeRepository.GetEpisodesByContentIdAsync(contentId);
            return episodes.Select(MapEpisodeToEpisodeResponseDto);
        }

        public async Task<IEnumerable<EpisodeResponseDto>> GetEpisodesBySeasonAsync(int contentId, int seasonNumber)
        {
            var episodes = await _episodeRepository.GetEpisodesBySeasonAsync(contentId, seasonNumber);
            return episodes.Select(MapEpisodeToEpisodeResponseDto);
        }

        public async Task<EpisodeResponseDto> GetEpisodeByContentAndNumberAsync(int contentId, int seasonNumber, int episodeNumber)
        {
            var episode = await _episodeRepository.GetEpisodeByContentAndNumberAsync(contentId, seasonNumber, episodeNumber);
            if (episode == null)
            {
                throw new KeyNotFoundException($"Episode with season {seasonNumber}, episode {episodeNumber} not found for content {contentId}");
            }

            return MapEpisodeToEpisodeResponseDto(episode);
        }

        public async Task<EpisodeResponseDto> CreateEpisodeAsync(CreateEpisodeDto createDto)
        {
            var content = await _contentRepository.GetByIdAsync(createDto.ContentId);
            if (content == null)
            {
                throw new KeyNotFoundException($"Content with ID {createDto.ContentId} not found");
            }

            if (content.ContentType != "TV Show")
            {
                throw new InvalidOperationException("Episodes can only be added to TV Shows");
            }

            var episode = new Episode
            {
                ContentId = createDto.ContentId,
                Title = createDto.Title,
                Description = createDto.Description,
                SeasonNumber = createDto.SeasonNumber,
                EpisodeNumber = createDto.EpisodeNumber,
                Duration = createDto.Duration,
                ThumbnailUrl = createDto.ThumbnailUrl,
                VideoUrl = createDto.VideoUrl
            };

            var createdEpisode = await _episodeRepository.AddAsync(episode);
            return MapEpisodeToEpisodeResponseDto(createdEpisode);
        }

        public async Task<EpisodeResponseDto> UpdateEpisodeAsync(int episodeId, UpdateEpisodeDto updateDto)
        {
            var episode = await _episodeRepository.GetByIdAsync(episodeId);
            if (episode == null)
            {
                throw new KeyNotFoundException($"Episode with ID {episodeId} not found");
            }

            if (!string.IsNullOrEmpty(updateDto.Title))
            {
                episode.Title = updateDto.Title;
            }

            if (!string.IsNullOrEmpty(updateDto.Description))
            {
                episode.Description = updateDto.Description;
            }

            if (!string.IsNullOrEmpty(updateDto.Duration))
            {
                episode.Duration = updateDto.Duration;
            }

            if (!string.IsNullOrEmpty(updateDto.ThumbnailUrl))
            {
                episode.ThumbnailUrl = updateDto.ThumbnailUrl;
            }

            if (!string.IsNullOrEmpty(updateDto.VideoUrl))
            {
                episode.VideoUrl = updateDto.VideoUrl;
            }

            episode.UpdatedAt = DateTime.UtcNow;

            var updatedEpisode = await _episodeRepository.UpdateAsync(episode);
            return MapEpisodeToEpisodeResponseDto(updatedEpisode);
        }

        public async Task<bool> DeleteEpisodeAsync(int episodeId)
        {
            return await _episodeRepository.SoftDeleteAsync(episodeId);
        }



        private EpisodeResponseDto MapEpisodeToEpisodeResponseDto(Episode episode)
        {
            return new EpisodeResponseDto
            {
                Id = episode.Id,
                ContentId = episode.ContentId,
                Title = episode.Title,
                Description = episode.Description,
                SeasonNumber = episode.SeasonNumber,
                EpisodeNumber = episode.EpisodeNumber,
                Duration = episode.Duration,
                ThumbnailUrl = episode.ThumbnailUrl,
                VideoUrl = episode.VideoUrl
            };
        }


    }
}
