using NetflixClone.DTOs;
using System.Threading.Tasks;

namespace NetflixClone.Interfaces
{
    public interface IUserService
    {
        Task<AuthResponseDto> RegisterUserAsync(UserRegistrationDto registrationDto);
        Task<AuthResponseDto> LoginUserAsync(UserLoginDto loginDto);
        Task<UserResponseDto> GetUserByIdAsync(int userId);
        Task<UserResponseDto?> GetUserByEmailAsync(string email);
        Task<UserResponseDto> UpdateUserAsync(int userId, UserUpdateDto updateDto);
        Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto);
        Task<bool> ChangeSubscriptionPlanAsync(int userId, ChangeSubscriptionDto changeSubscriptionDto);
        Task<bool> DeactivateUserAsync(int userId);
        Task<bool> DeleteUserAsync(int userId);
        Task<bool> UpdatePasswordAsync(string email, string newPassword);
        Task<string> GenerateJwtTokenAsync(Models.User user);
        Task<UserResponseDto> MapUserToUserResponseDto(Models.User user);
        
    }
}
