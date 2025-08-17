using NetflixClone.Data;
using NetflixClone.DTOs;
using NetflixClone.Interfaces;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using Microsoft.Extensions.Logging;

namespace NetflixClone.Services
{
    public class TwoFactorAuthService : ITwoFactorAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly ICodeGenerationService _codeService;
        private readonly IUserService _userService;
        private readonly ILogger<TwoFactorAuthService> _logger;

        public TwoFactorAuthService(
            ApplicationDbContext context,
            IEmailService emailService,
            ICodeGenerationService codeService,
            IUserService userService,
            ILogger<TwoFactorAuthService> logger)
        {
            _context = context;
            _emailService = emailService;
            _codeService = codeService;
            _userService = userService;
            _logger = logger;
        }

        public async Task<TwoFactorAuthResponseDto> EnableTwoFactorAsync(string email)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
                if (user == null)
                {
                    return new TwoFactorAuthResponseDto
                    {
                        Success = false,
                        Message = "User not found"
                    };
                }

                var existing2FA = await _context.TwoFactorAuths.FirstOrDefaultAsync(t => t.UserId == user.Id);
                if (existing2FA != null && existing2FA.IsEnabled)
                {
                    return new TwoFactorAuthResponseDto
                    {
                        Success = false,
                        Message = "Two-factor authentication is already enabled"
                    };
                }

                if (existing2FA != null)
                {
                    existing2FA.IsEnabled = true;
                    existing2FA.UpdatedAt = DateTime.UtcNow;
                }
                else
                {
                    var twoFactorAuth = new Models.TwoFactorAuth
                    {
                        UserId = user.Id,
                        IsEnabled = true,
                        Email = email,
                        IsVerified = false
                    };
                    _context.TwoFactorAuths.Add(twoFactorAuth);
                }

                await _context.SaveChangesAsync();

                return new TwoFactorAuthResponseDto
                {
                    Success = true,
                    Message = "Two-factor authentication enabled successfully. You will receive verification codes during login.",
                    IsEnabled = true,
                    Email = email
                };
            }
            catch (Exception ex)
            {

                return new TwoFactorAuthResponseDto
                {
                    Success = false,
                    Message = "An error occurred while enabling two-factor authentication"
                };
            }
        }

        public async Task<TwoFactorAuthResponseDto> DisableTwoFactorAsync(string email, string password)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
                if (user == null)
                {
                                    return new TwoFactorAuthResponseDto
                    {
                        Success = false,
                        Message = "User not found"
                    };
                }

                var passwordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
                
                if (!passwordValid)
                {
                                    return new TwoFactorAuthResponseDto
                    {
                        Success = false,
                        Message = "Invalid password"
                    };
                }

                var twoFactorAuth = await _context.TwoFactorAuths.FirstOrDefaultAsync(t => t.UserId == user.Id);
                if (twoFactorAuth == null || !twoFactorAuth.IsEnabled)
                {
                                    return new TwoFactorAuthResponseDto 
                    {
                        Success = false,
                        Message = "Two-factor authentication is not enabled"
                    };
                }

                _context.TwoFactorAuths.Update(twoFactorAuth);
                
                twoFactorAuth.IsEnabled = false;
                twoFactorAuth.UpdatedAt = DateTime.UtcNow;
                
                await _context.SaveChangesAsync();

                return new TwoFactorAuthResponseDto
                {
                    Success = true,
                    Message = "Two-factor authentication has been disabled",
                    IsEnabled = false,
                    Email = email
                };
            }
            catch (Exception ex)
            {

                return new TwoFactorAuthResponseDto
                {
                    Success = false,
                    Message = $"An error occurred while disabling two-factor authentication: {ex.Message}"
                };
            }
        }

        public async Task<TwoFactorAuthResponseDto> GetTwoFactorStatusAsync(string email)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
                if (user == null)
                {
                    return new TwoFactorAuthResponseDto
                    {
                        Success = false,
                        Message = "User not found"
                    };
                }

                var twoFactorAuth = await _context.TwoFactorAuths.FirstOrDefaultAsync(t => t.UserId == user.Id);
                var isEnabled = twoFactorAuth?.IsEnabled ?? false;

                return new TwoFactorAuthResponseDto
                {
                    Success = true,
                    Message = isEnabled ? "Two-factor authentication is enabled" : "Two-factor authentication is disabled",
                    IsEnabled = isEnabled,
                    Email = email
                };
            }
            catch (Exception ex)
            {

                return new TwoFactorAuthResponseDto
                {
                    Success = false,
                    Message = "An error occurred while getting two-factor authentication status"
                };
            }
        }

        public async Task<TwoFactorVerificationResponseDto> VerifyTwoFactorCodeAsync(string email, string code)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
                if (user == null)
                {
                    return new TwoFactorVerificationResponseDto
                    {
                        Success = false,
                        Message = "User not found"
                    };
                }

                var twoFactorAuth = await _context.TwoFactorAuths.FirstOrDefaultAsync(t => t.UserId == user.Id);
                if (twoFactorAuth == null || !twoFactorAuth.IsEnabled)
                {
                    return new TwoFactorVerificationResponseDto
                    {
                        Success = false,
                        Message = "Two-factor authentication is not enabled"
                    };
                }

                if (DateTime.UtcNow > twoFactorAuth.CodeExpiresAt)
                {
                    return new TwoFactorVerificationResponseDto
                    {
                        Success = false,
                        Message = "Verification code has expired"
                    };
                }

                if (twoFactorAuth.VerificationCode != code)
                {
                    return new TwoFactorVerificationResponseDto
                    {
                        Success = false,
                        Message = "Invalid verification code"
                    };
                }

                twoFactorAuth.IsVerified = true;
                twoFactorAuth.LastUsedAt = DateTime.UtcNow;
                twoFactorAuth.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                var token = await _userService.GenerateJwtTokenAsync(user);
                var userResponse = await _userService.MapUserToUserResponseDto(user);

                return new TwoFactorVerificationResponseDto
                {
                    Success = true,
                    Message = "Two-factor authentication verified successfully",
                    IsVerified = true,
                    Token = token,
                    User = userResponse,
                    Role = user.Role
                };
            }
            catch (Exception ex)
            {

                return new TwoFactorVerificationResponseDto
                {
                    Success = false,
                    Message = "An error occurred while verifying the code"
                };
            }
        }

        public async Task<bool> IsTwoFactorEnabledAsync(string email)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
                if (user == null) return false;

                var twoFactorAuth = await _context.TwoFactorAuths.FirstOrDefaultAsync(t => t.UserId == user.Id);
                return twoFactorAuth?.IsEnabled ?? false;
            }
            catch (Exception ex)
            {

                return false;
            }
        }

        public async Task<bool> SendVerificationCodeAsync(string email)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
                if (user == null) return false;

                var twoFactorAuth = await _context.TwoFactorAuths.FirstOrDefaultAsync(t => t.UserId == user.Id);
                if (twoFactorAuth == null || !twoFactorAuth.IsEnabled) return false;

                var verificationCode = _codeService.GenerateSecureCode();
                var expiresAt = DateTime.UtcNow.AddMinutes(10);

                twoFactorAuth.VerificationCode = verificationCode;
                twoFactorAuth.CodeGeneratedAt = DateTime.UtcNow;
                twoFactorAuth.CodeExpiresAt = expiresAt;
                twoFactorAuth.IsVerified = false;
                twoFactorAuth.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return await _emailService.SendTwoFactorCodeAsync(email, verificationCode);
            }
            catch (Exception ex)
            {

                return false;
            }
        }
    }
}
