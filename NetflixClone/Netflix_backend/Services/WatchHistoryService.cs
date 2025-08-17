using NetflixClone.DTOs;
using NetflixClone.Models;
using NetflixClone.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetflixClone.Services
{
    public class WatchHistoryService : IWatchHistoryService
    {
        private readonly IWatchHistoryRepository _watchHistoryRepository;
        private readonly IUserRepository _userRepository;
        private readonly IProfileRepository _profileRepository;
        private readonly IContentRepository _contentRepository;
        private readonly IEpisodeRepository _episodeRepository;

        public WatchHistoryService(
            IWatchHistoryRepository watchHistoryRepository,
            IUserRepository userRepository,
            IProfileRepository profileRepository,
            IContentRepository contentRepository,
            IEpisodeRepository episodeRepository)
        {
            _watchHistoryRepository = watchHistoryRepository;
            _userRepository = userRepository;
            _profileRepository = profileRepository;
            _contentRepository = contentRepository;
            _episodeRepository = episodeRepository;
        }

        public async Task<IEnumerable<WatchHistoryResponseDto>> GetWatchHistoryByProfileIdAsync(int profileId)
        {
            var watchHistories = await _watchHistoryRepository.GetWatchHistoryByProfileIdAsync(profileId);
            var result = new List<WatchHistoryResponseDto>();

            foreach (var history in watchHistories)
            {
                var dto = await MapWatchHistoryToResponseDto(history);
                result.Add(dto);
            }

            return result;
        }


        public async Task<WatchHistoryResponseDto> GetWatchHistoryByIdAsync(int watchHistoryId)
        {
            var watchHistory = await _watchHistoryRepository.GetByIdAsync(watchHistoryId);
            if (watchHistory == null)
            {
                throw new KeyNotFoundException($"Watch history with ID {watchHistoryId} not found");
            }

            return await MapWatchHistoryToResponseDto(watchHistory);
        }

        public async Task<WatchHistoryResponseDto> GetWatchHistoryByContentAndProfileAsync(int contentId, int profileId, int? episodeId = null)
        {
            var watchHistory = await _watchHistoryRepository.GetWatchHistoryByContentAndProfileAsync(contentId, profileId, episodeId);
            if (watchHistory == null)
            {
                throw new KeyNotFoundException($"Watch history not found for content {contentId} and profile {profileId}");
            }

            return await MapWatchHistoryToResponseDto(watchHistory);
        }

        public async Task<WatchHistoryResponseDto> CreateOrUpdateWatchHistoryAsync(WatchHistoryCreateDto createDto)
        {
            var profile = await _profileRepository.GetByIdAsync(createDto.ProfileId);
            if (profile == null)
            {
                throw new KeyNotFoundException($"Profile with ID {createDto.ProfileId} not found");
            }

            var content = await _contentRepository.GetByIdAsync(createDto.ContentId);
            if (content == null)
            {
                throw new KeyNotFoundException($"Content with ID {createDto.ContentId} not found");
            }

            if (createDto.EpisodeId.HasValue)
            {
                var episode = await _episodeRepository.GetByIdAsync(createDto.EpisodeId.Value);
                if (episode == null)
                {
                    throw new KeyNotFoundException($"Episode with ID {createDto.EpisodeId.Value} not found");
                }
            }

            var existingWatchHistory = await _watchHistoryRepository.GetWatchHistoryByContentAndProfileAsync(
                createDto.ContentId, createDto.ProfileId, createDto.EpisodeId);

            if (existingWatchHistory != null)
            {
                existingWatchHistory.WatchedPercentage = createDto.WatchedPercentage;
                existingWatchHistory.WatchedDuration = createDto.WatchedDuration;
                existingWatchHistory.IsCompleted = createDto.IsCompleted;
                existingWatchHistory.LastWatched = DateTime.UtcNow;
                existingWatchHistory.UpdatedAt = DateTime.UtcNow;

                var updatedWatchHistory = await _watchHistoryRepository.UpdateAsync(existingWatchHistory);
                return await MapWatchHistoryToResponseDto(updatedWatchHistory);
            }
            else
            {
                var watchHistory = new WatchHistory
                {
                    UserId = profile.UserId,
                    ProfileId = createDto.ProfileId,
                    ContentId = createDto.ContentId,
                    EpisodeId = createDto.EpisodeId,
                    WatchedPercentage = createDto.WatchedPercentage,
                    WatchedDuration = createDto.WatchedDuration,
                    IsCompleted = createDto.IsCompleted,
                    LastWatched = DateTime.UtcNow
                };

                var createdWatchHistory = await _watchHistoryRepository.AddAsync(watchHistory);
                return await MapWatchHistoryToResponseDto(createdWatchHistory);
            }
        }

        public async Task<WatchHistoryResponseDto> UpdateWatchHistoryAsync(int watchHistoryId, WatchHistoryUpdateDto updateDto)
        {
            var watchHistory = await _watchHistoryRepository.GetByIdAsync(watchHistoryId);
            if (watchHistory == null)
            {
                throw new KeyNotFoundException($"Watch history with ID {watchHistoryId} not found");
            }

            watchHistory.WatchedPercentage = updateDto.WatchedPercentage;
            watchHistory.WatchedDuration = updateDto.WatchedDuration;
            watchHistory.IsCompleted = updateDto.IsCompleted;
            watchHistory.LastWatched = DateTime.UtcNow;
            watchHistory.UpdatedAt = DateTime.UtcNow;

            var updatedWatchHistory = await _watchHistoryRepository.UpdateAsync(watchHistory);
            return await MapWatchHistoryToResponseDto(updatedWatchHistory);
        }

        public async Task<bool> DeleteWatchHistoryAsync(int watchHistoryId)
        {
            return await _watchHistoryRepository.SoftDeleteAsync(watchHistoryId);
        }

        public async Task<IEnumerable<ContinueWatchingDto>> GetContinueWatchingAsync(int profileId, int limit = 10)
        {
            var watchHistories = await _watchHistoryRepository.GetContinueWatchingAsync(profileId, limit);
            var result = new List<ContinueWatchingDto>();

            foreach (var watchHistory in watchHistories)
            {
                result.Add(new ContinueWatchingDto
                {
                    ContentId = watchHistory.ContentId,
                    EpisodeId = watchHistory.EpisodeId,
                    Title = watchHistory.Episode != null 
                        ? $"{watchHistory.Content.Title}: S{watchHistory.Episode.SeasonNumber}E{watchHistory.Episode.EpisodeNumber} - {watchHistory.Episode.Title}"
                        : watchHistory.Content.Title,
                    ThumbnailUrl = watchHistory.Episode != null && !string.IsNullOrEmpty(watchHistory.Episode.ThumbnailUrl)
                        ? watchHistory.Episode.ThumbnailUrl
                        : watchHistory.Content.ThumbnailUrl,
                    ContentType = watchHistory.Content.ContentType,
                    WatchedPercentage = watchHistory.WatchedPercentage,
                    LastWatched = watchHistory.LastWatched
                });
            }

            return result;
        }

        public async Task<IEnumerable<WatchHistoryResponseDto>> GetCompletedWatchHistoryAsync(int profileId)
        {
            var watchHistories = await _watchHistoryRepository.GetWatchHistoryByProfileIdAsync(profileId);
            var completedHistories = watchHistories.Where(wh => wh.IsCompleted);
            var result = new List<WatchHistoryResponseDto>();

            foreach (var history in completedHistories)
            {
                var dto = await MapWatchHistoryToResponseDto(history);
                result.Add(dto);
            }

            return result;
        }



        private async Task<WatchHistoryResponseDto> MapWatchHistoryToResponseDto(WatchHistory watchHistory)
        {
            var content = await _contentRepository.GetByIdAsync(watchHistory.ContentId);
            Episode? episode = null;

            if (watchHistory.EpisodeId.HasValue)
            {
                episode = await _episodeRepository.GetByIdAsync(watchHistory.EpisodeId.Value);
            }

            var dto = new WatchHistoryResponseDto
            {
                Id = watchHistory.Id,
                UserId = watchHistory.UserId,
                ProfileId = watchHistory.ProfileId,
                ContentId = watchHistory.ContentId,
                EpisodeId = watchHistory.EpisodeId,
                WatchedPercentage = watchHistory.WatchedPercentage,
                LastWatched = watchHistory.LastWatched,
                WatchedDuration = watchHistory.WatchedDuration,
                IsCompleted = watchHistory.IsCompleted,
                Content = content != null ? MapContentToContentSummaryDto(content) : new ContentSummaryDto(),
                Episode = episode != null ? MapEpisodeToEpisodeResponseDto(episode) : null
            };

            return dto;
        }

       

        private ContentSummaryDto MapContentToContentSummaryDto(Content content)
        {
            return new ContentSummaryDto
            {
                Id = content.Id,
                Title = content.Title,
                Description = content.Description,
                ReleaseYear = content.ReleaseYear,
                MaturityRating = content.MaturityRating,
                Duration = content.Duration,
                TotalSeasons = content.TotalSeasons,
                ThumbnailUrl = content.ThumbnailUrl,
                CoverImageUrl = content.CoverImageUrl,
                VideoUrl = content.VideoUrl,
                ContentType = content.ContentType,
                Genre = content.Genre,
                Director = content.Director,
                Cast = content.Cast,
                CreatedAt = content.CreatedAt,
                UpdatedAt = content.UpdatedAt ?? content.CreatedAt
            };
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
