using NetflixClone.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetflixClone.Interfaces
{
    public interface IProfileService
    {
        Task<IEnumerable<ProfileResponseDto>> GetProfilesByUserIdAsync(int userId);
        Task<ProfileResponseDto> GetProfileByIdAsync(int profileId);
        Task<ProfileResponseDto> CreateProfileAsync(int userId, ProfileCreateDto createDto);
        Task<ProfileResponseDto> UpdateProfileAsync(int profileId, ProfileUpdateDto updateDto);
        Task<bool> DeleteProfileAsync(int profileId);
        Task<int> GetProfileCountByUserIdAsync(int userId);
    }
}
