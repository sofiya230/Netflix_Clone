using NetflixClone.DTOs;
using NetflixClone.Models;
using NetflixClone.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetflixClone.Services
{
    public class ContentService : IContentService
    {
        private readonly IContentRepository _contentRepository;
        private readonly IEpisodeRepository _episodeRepository;

        public ContentService(IContentRepository contentRepository, IEpisodeRepository episodeRepository)
        {
            _contentRepository = contentRepository;
            _episodeRepository = episodeRepository;
        }

        public async Task<ContentResponseDto> GetContentByIdAsync(int contentId)
        {
            var content = await _contentRepository.GetContentWithEpisodesAsync(contentId);
            if (content == null)
            {
                throw new KeyNotFoundException($"Content with ID {contentId} not found");
            }

            return MapContentToContentResponseDto(content);
        }

        public async Task<IEnumerable<ContentSummaryDto>> GetAllContentsAsync()
        {
            var contents = await _contentRepository.GetAllAsync();
            return contents.Select(MapContentToContentSummaryDto);
        }

        public async Task<IEnumerable<ContentSummaryDto>> GetContentsByGenreAsync(string genre)
        {
            var contents = await _contentRepository.GetContentByGenreAsync(genre);
            return contents.Select(MapContentToContentSummaryDto);
        }

        public async Task<IEnumerable<ContentSummaryDto>> GetContentsByTypeAsync(string contentType)
        {
            var contents = await _contentRepository.GetContentByTypeAsync(contentType);
            return contents.Select(MapContentToContentSummaryDto);
        }

        public async Task<ContentResponseDto> CreateContentAsync(CreateContentDto createDto)
        {
            var content = new Content
            {
                Title = createDto.Title,
                Description = createDto.Description,
                ReleaseYear = createDto.ReleaseYear,
                MaturityRating = createDto.MaturityRating,
                Duration = createDto.Duration,
                TotalSeasons = createDto.TotalSeasons,
                ThumbnailUrl = createDto.ThumbnailUrl,
                CoverImageUrl = createDto.CoverImageUrl,
                VideoUrl = createDto.VideoUrl,
                ContentType = createDto.ContentType,
                Genre = createDto.Genre,
                Director = createDto.Director,
                Cast = createDto.Cast
            };

            var createdContent = await _contentRepository.AddAsync(content);
            return MapContentToContentResponseDto(createdContent);
        }

        public async Task<ContentResponseDto> UpdateContentAsync(int contentId, UpdateContentDto updateDto)
        {
            var content = await _contentRepository.GetByIdAsync(contentId);
            if (content == null)
            {
                throw new KeyNotFoundException($"Content with ID {contentId} not found");
            }

            if (!string.IsNullOrEmpty(updateDto.Title))
            {
                content.Title = updateDto.Title;
            }

            if (!string.IsNullOrEmpty(updateDto.Description))
            {
                content.Description = updateDto.Description;
            }

            if (updateDto.ReleaseYear.HasValue)
            {
                content.ReleaseYear = updateDto.ReleaseYear.Value;
            }

            if (!string.IsNullOrEmpty(updateDto.MaturityRating))
            {
                content.MaturityRating = updateDto.MaturityRating;
            }

            if (!string.IsNullOrEmpty(updateDto.Duration))
            {
                content.Duration = updateDto.Duration;
            }

            if (updateDto.TotalSeasons.HasValue)
            {
                content.TotalSeasons = updateDto.TotalSeasons;
            }

            if (!string.IsNullOrEmpty(updateDto.ThumbnailUrl))
            {
                content.ThumbnailUrl = updateDto.ThumbnailUrl;
            }

            if (!string.IsNullOrEmpty(updateDto.CoverImageUrl))
            {
                content.CoverImageUrl = updateDto.CoverImageUrl;
            }

            if (!string.IsNullOrEmpty(updateDto.VideoUrl))
            {
                content.VideoUrl = updateDto.VideoUrl;
            }

            if (!string.IsNullOrEmpty(updateDto.Genre))
            {
                content.Genre = updateDto.Genre;
            }

            if (!string.IsNullOrEmpty(updateDto.Director))
            {
                content.Director = updateDto.Director;
            }

            if (!string.IsNullOrEmpty(updateDto.Cast))
            {
                content.Cast = updateDto.Cast;
            }

            content.UpdatedAt = DateTime.UtcNow;

            var updatedContent = await _contentRepository.UpdateAsync(content);
            
            var contentWithEpisodes = await _contentRepository.GetContentWithEpisodesAsync(contentId);
            return MapContentToContentResponseDto(contentWithEpisodes!);
        }

        public async Task<bool> DeleteContentAsync(int contentId)
        {
            return await _contentRepository.SoftDeleteAsync(contentId);
        }

        public async Task<IEnumerable<ContentSummaryDto>> GetTrendingContentAsync(int limit = 10)
        {
            var contents = await _contentRepository.GetTrendingContentAsync(limit);
            return contents.Select(MapContentToContentSummaryDto);
        }

        public async Task<IEnumerable<ContentSummaryDto>> GetNewReleasesAsync(int limit = 10)
        {
            var contents = await _contentRepository.GetNewReleasesAsync(limit);
            return contents.Select(MapContentToContentSummaryDto);
        }

        public async Task<IEnumerable<ContentSummaryDto>> SearchContentsAsync(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return new List<ContentSummaryDto>();
            }

            var contents = await _contentRepository.FindAsync(c => 
                c.Title.Contains(searchTerm) ||
                c.Description.Contains(searchTerm) ||
                c.Cast.Contains(searchTerm) ||
                c.Director.Contains(searchTerm));

            return contents.Select(MapContentToContentSummaryDto);
        }



        private ContentResponseDto MapContentToContentResponseDto(Content content)
        {
            var contentDto = new ContentResponseDto
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
                Episodes = new List<EpisodeResponseDto>()
            };

            if (content.Episodes != null && content.Episodes.Count > 0)
            {
                foreach (var episode in content.Episodes)
                {
                    contentDto.Episodes.Add(new EpisodeResponseDto
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
                    });
                }
            }

            return contentDto;
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


    }
}
