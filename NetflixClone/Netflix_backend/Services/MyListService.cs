using NetflixClone.DTOs;
using NetflixClone.Models;
using NetflixClone.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetflixClone.Services
{
    public class MyListService : IMyListService
    {
        private readonly IMyListRepository _myListRepository;
        private readonly IProfileRepository _profileRepository;
        private readonly IContentRepository _contentRepository;

        public MyListService(
            IMyListRepository myListRepository,
            IProfileRepository profileRepository,
            IContentRepository contentRepository)
        {
            _myListRepository = myListRepository;
            _profileRepository = profileRepository;
            _contentRepository = contentRepository;
        }

        public async Task<IEnumerable<MyListResponseDto>> GetMyListByProfileIdAsync(int profileId)
        {
            var myList = await _myListRepository.GetMyListByProfileIdAsync(profileId);
            return myList.Select(MapMyListToResponseDto);
        }

        public async Task<MyListResponseDto> AddToMyListAsync(MyListCreateDto createDto)
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

            var isInMyList = await _myListRepository.IsContentInMyListAsync(createDto.ContentId, createDto.ProfileId);
            if (isInMyList)
            {
                var existingItem = await _myListRepository.GetMyListItemAsync(createDto.ContentId, createDto.ProfileId);
                if (existingItem != null)
                {
                    return await MapMyListToResponseDtoWithContent(existingItem);
                }
            }

            var myList = new MyList
            {
                UserId = profile.UserId,
                ProfileId = createDto.ProfileId,
                ContentId = createDto.ContentId,
                AddedAt = DateTime.UtcNow
            };

            var createdMyList = await _myListRepository.AddAsync(myList);
            return await MapMyListToResponseDtoWithContent(createdMyList);
        }

        public async Task<bool> RemoveFromMyListAsync(int contentId, int profileId)
        {
            return await _myListRepository.RemoveFromMyListAsync(contentId, profileId);
        }

        public async Task<bool> IsContentInMyListAsync(int contentId, int profileId)
        {
            return await _myListRepository.IsContentInMyListAsync(contentId, profileId);
        }



        private MyListResponseDto MapMyListToResponseDto(MyList myList)
        {
            var dto = new MyListResponseDto
            {
                Id = myList.Id,
                UserId = myList.UserId,
                ProfileId = myList.ProfileId,
                ContentId = myList.ContentId,
                AddedAt = myList.AddedAt
            };

            if (myList.Content != null)
            {
                dto.Content = MapContentToContentSummaryDto(myList.Content);
            }

            return dto;
        }

        private async Task<MyListResponseDto> MapMyListToResponseDtoWithContent(MyList myList)
        {
            var content = await _contentRepository.GetByIdAsync(myList.ContentId);
            
            var dto = new MyListResponseDto
            {
                Id = myList.Id,
                UserId = myList.UserId,
                ProfileId = myList.ProfileId,
                ContentId = myList.ContentId,
                AddedAt = myList.AddedAt,
                Content = content != null ? MapContentToContentSummaryDto(content) : new ContentSummaryDto()
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


    }
}
