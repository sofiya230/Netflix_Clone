using NetflixClone.Data;
using NetflixClone.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace NetflixClone.Services
{
    public interface IPasswordResetService
    {
        Task<string> CreatePasswordResetAsync(string email);
        Task<bool> VerifyResetCodeAsync(string email, string resetCode);
        Task<bool> MarkResetCodeAsUsedAsync(string email, string resetCode);
        Task<bool> IsResetCodeExpiredAsync(string email, string resetCode);
        Task CleanupExpiredResetCodesAsync();
    }

    public class PasswordResetService : IPasswordResetService
    {
        private readonly ApplicationDbContext _context;
        private readonly ICodeGenerationService _codeService;
        private readonly TimeSpan _resetCodeExpiry = TimeSpan.FromMinutes(10);

        public PasswordResetService(ApplicationDbContext context, ICodeGenerationService codeService)
        {
            _context = context;
            _codeService = codeService;
        }

        public async Task<string> CreatePasswordResetAsync(string email)
        {
            try
            {
                var existingResets = await _context.PasswordResets
                    .Where(pr => pr.Email == email)
                    .ToListAsync();

                _context.PasswordResets.RemoveRange(existingResets);

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null)
                {
                    return null;
                }

                var resetCode = _codeService.GenerateSecureCode();
                
                var passwordReset = new PasswordReset
                {
                    UserId = user.Id,
                    Email = email,
                    ResetCode = resetCode,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.Add(_resetCodeExpiry),
                    IsUsed = false
                };

                await _context.PasswordResets.AddAsync(passwordReset);
                await _context.SaveChangesAsync();

                return resetCode;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> VerifyResetCodeAsync(string email, string resetCode)
        {
            try
            {
                var passwordReset = await _context.PasswordResets
                    .FirstOrDefaultAsync(pr => pr.Email == email && 
                                             pr.ResetCode == resetCode && 
                                             !pr.IsUsed);

                if (passwordReset == null)
                    return false;

                if (_codeService.IsCodeExpired(passwordReset.CreatedAt))
                    return false;

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> MarkResetCodeAsUsedAsync(string email, string resetCode)
        {
            try
            {
                var passwordReset = await _context.PasswordResets
                    .FirstOrDefaultAsync(pr => pr.Email == email && 
                                             pr.ResetCode == resetCode);

                if (passwordReset == null)
                    return false;

                passwordReset.IsUsed = true;
                passwordReset.UsedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> IsResetCodeExpiredAsync(string email, string resetCode)
        {
            try
            {
                var passwordReset = await _context.PasswordResets
                    .FirstOrDefaultAsync(pr => pr.Email == email && 
                                             pr.ResetCode == resetCode);

                if (passwordReset == null)
                    return true;

                return _codeService.IsCodeExpired(passwordReset.CreatedAt);
            }
            catch
            {
                return true;
            }
        }

        public async Task CleanupExpiredResetCodesAsync()
        {
            try
            {
                var expiredCodes = await _context.PasswordResets
                    .Where(pr => DateTime.UtcNow > pr.ExpiresAt)
                    .ToListAsync();

                _context.PasswordResets.RemoveRange(expiredCodes);
                await _context.SaveChangesAsync();
            }
            catch
            {
            }
        }
    }
}
