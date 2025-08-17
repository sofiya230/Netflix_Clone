using NetflixClone.DTOs;

namespace NetflixClone.Interfaces
{
    public interface ITwoFactorAuthService
    {
        Task<TwoFactorAuthResponseDto> EnableTwoFactorAsync(string email);
        Task<TwoFactorAuthResponseDto> DisableTwoFactorAsync(string email, string password);
        Task<TwoFactorAuthResponseDto> GetTwoFactorStatusAsync(string email);
        Task<TwoFactorVerificationResponseDto> VerifyTwoFactorCodeAsync(string email, string code);
        Task<bool> IsTwoFactorEnabledAsync(string email);
        Task<bool> SendVerificationCodeAsync(string email);
    }
}


