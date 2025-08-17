using NetflixClone.DTOs;
using NetflixClone.Models;
using NetflixClone.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetflixClone.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IProfileRepository _profileRepository;
        private readonly IUserRepository _userRepository;
        private readonly int _maxProfilesPerUser = 5;

        public ProfileService(IProfileRepository profileRepository, IUserRepository userRepository)
        {
            _profileRepository = profileRepository;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<ProfileResponseDto>> GetProfilesByUserIdAsync(int userId)
        {
            var profiles = await _profileRepository.GetProfilesByUserIdAsync(userId);
            return profiles.Select(MapProfileToProfileResponseDto);
        }

        public async Task<ProfileResponseDto> GetProfileByIdAsync(int profileId)
        {
            var profile = await _profileRepository.GetByIdAsync(profileId);
            if (profile == null)
            {
                throw new KeyNotFoundException($"Profile with ID {profileId} not found");
            }

            return MapProfileToProfileResponseDto(profile);
        }

        public async Task<ProfileResponseDto> CreateProfileAsync(int userId, ProfileCreateDto createDto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {userId} not found");
            }

            var profileCount = await _profileRepository.GetProfileCountByUserIdAsync(userId);
            if (profileCount >= _maxProfilesPerUser)
            {
                throw new InvalidOperationException($"Maximum number of profiles ({_maxProfilesPerUser}) reached");
            }

            var profile = new Profile
            {
                UserId = userId,
                Name = createDto.Name,
                AvatarUrl = string.IsNullOrEmpty(createDto.AvatarUrl) ? "/images/defaultavatar.jpg" : createDto.AvatarUrl,
                IsKidsProfile = createDto.IsKidsProfile,
                Language = createDto.Language,
                MaturityLevel = createDto.IsKidsProfile ? "Kids" : createDto.MaturityLevel
            };

            var createdProfile = await _profileRepository.AddAsync(profile);
            return MapProfileToProfileResponseDto(createdProfile);
        }

        public async Task<ProfileResponseDto> UpdateProfileAsync(int profileId, ProfileUpdateDto updateDto)
        {
            var profile = await _profileRepository.GetByIdAsync(profileId);
            if (profile == null)
            {
                throw new KeyNotFoundException($"Profile with ID {profileId} not found");
            }

            if (!string.IsNullOrEmpty(updateDto.Name))
            {
                profile.Name = updateDto.Name;
            }

            if (!string.IsNullOrEmpty(updateDto.AvatarUrl))
            {
                profile.AvatarUrl = updateDto.AvatarUrl;
            }

            if (!string.IsNullOrEmpty(updateDto.Language))
            {
                profile.Language = updateDto.Language;
            }

            if (!string.IsNullOrEmpty(updateDto.MaturityLevel) && !profile.IsKidsProfile)
            {
                profile.MaturityLevel = updateDto.MaturityLevel;
            }

            profile.UpdatedAt = DateTime.UtcNow;

            var updatedProfile = await _profileRepository.UpdateAsync(profile);
            return MapProfileToProfileResponseDto(updatedProfile);
        }

        public async Task<bool> DeleteProfileAsync(int profileId)
        {
            return await _profileRepository.SoftDeleteAsync(profileId);
        }

        public async Task<int> GetProfileCountByUserIdAsync(int userId)
        {
            return await _profileRepository.GetProfileCountByUserIdAsync(userId);
        }



        private ProfileResponseDto MapProfileToProfileResponseDto(Profile profile)
        {
            return new ProfileResponseDto
            {
                Id = profile.Id,
                UserId = profile.UserId,
                Name = profile.Name,
                AvatarUrl = profile.AvatarUrl,
                IsKidsProfile = profile.IsKidsProfile,
                Language = profile.Language,
                MaturityLevel = profile.MaturityLevel
            };
        }


    }
}
