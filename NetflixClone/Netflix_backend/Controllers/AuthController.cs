
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetflixClone.DTOs;
using NetflixClone.Interfaces;
using NetflixClone.Services;
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Collections.Generic;

namespace NetflixClone.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;
        private readonly IPasswordResetService _passwordResetService;
        private readonly ITwoFactorAuthService _twoFactorAuthService;

        public AuthController(
            IUserService userService, 
            IEmailService emailService, 
            IPasswordResetService passwordResetService,
            ITwoFactorAuthService twoFactorAuthService)
        {
            _userService = userService;
            _emailService = emailService;
            _passwordResetService = passwordResetService;
            _twoFactorAuthService = twoFactorAuthService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register(UserRegistrationDto registrationDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userService.RegisterUserAsync(registrationDto);

            if (!result.IsSuccessful)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(UserLoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userService.LoginUserAsync(loginDto);

            if (!result.IsSuccessful)
            {
                return Unauthorized(result);
            }

            var is2FAEnabled = await _twoFactorAuthService.IsTwoFactorEnabledAsync(loginDto.Email);
            
            if (is2FAEnabled)
            {
                var codeSent = await _twoFactorAuthService.SendVerificationCodeAsync(loginDto.Email);
                
                if (codeSent)
                {
                    return Ok(new AuthResponseDto
                    {
                        IsSuccessful = true,
                        Message = "Login successful. Two-factor authentication required. Please check your email for the verification code.",
                        RequiresTwoFactor = true,
                        Email = loginDto.Email
                    });
                }
                else
                {
                    return BadRequest(new AuthResponseDto
                    {
                        IsSuccessful = false,
                        Message = "Login successful but failed to send 2FA code. Please try again."
                    });
                }
            }

            return Ok(result); 
        }

        [HttpPost("forgot-password")]
        public async Task<ActionResult<PasswordResetResponseDto>> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var user = await _userService.GetUserByEmailAsync(dto.Email);
                
                if (user == null)
                {
                    return Ok(new PasswordResetResponseDto
                    {
                        Success = true,
                        Message = "If an account with that email exists, a password reset code has been sent.",
                        Email = dto.Email
                    });
                }

                var resetCode = await _passwordResetService.CreatePasswordResetAsync(dto.Email);
                
                if (string.IsNullOrEmpty(resetCode))
                {
                    return BadRequest(new PasswordResetResponseDto
                    {
                        Success = false,
                        Message = "Failed to generate reset code. Please try again."
                    });
                }
                
                var emailSent = await _emailService.SendPasswordResetEmailAsync(dto.Email, resetCode);
                
                if (emailSent)
                {
                    return Ok(new PasswordResetResponseDto
                    {
                        Success = true,
                        Message = "Password reset code has been sent to your email. Please check your inbox.",
                        Email = dto.Email
                    });
                }
                else
                {
                    return BadRequest(new PasswordResetResponseDto
                    {
                        Success = false,
                        Message = "Failed to send reset code. Please try again."
                    });
                }
            }
            catch (Exception)
            {
                return BadRequest(new PasswordResetResponseDto
                {
                    Success = false,
                    Message = "An error occurred. Please try again."
                });
            }
        }

        [HttpPost("verify-reset-code")]
        public async Task<ActionResult<PasswordResetResponseDto>> VerifyResetCode([FromBody] VerifyResetCodeDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                bool isValidCode = await _passwordResetService.VerifyResetCodeAsync(dto.Email, dto.ResetCode);
                
                if (isValidCode)
                {
                    return Ok(new PasswordResetResponseDto
                    {
                        Success = true,
                        Message = "Reset code verified successfully",
                        Email = dto.Email
                    });
                }
                else
                {
                    return BadRequest(new PasswordResetResponseDto
                    {
                        Success = false,
                        Message = "Invalid or expired reset code"
                    });
                }
            }
            catch (Exception)
            {
                return BadRequest(new PasswordResetResponseDto
                {
                    Success = false,
                    Message = "An error occurred. Please try again."
                });
            }
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult<PasswordResetResponseDto>> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                bool isValidCode = await _passwordResetService.VerifyResetCodeAsync(dto.Email, dto.ResetCode);
                
                if (!isValidCode)
                {
                    return BadRequest(new PasswordResetResponseDto
                    {
                        Success = false,
                        Message = "Invalid or expired reset code"
                    });
                }

                var passwordUpdated = await _userService.UpdatePasswordAsync(dto.Email, dto.NewPassword);
                
                if (passwordUpdated)
                {
                    await _passwordResetService.MarkResetCodeAsUsedAsync(dto.Email, dto.ResetCode);
                    
                    return Ok(new PasswordResetResponseDto
                    {
                        Success = true,
                        Message = "Password updated successfully"
                    });
                }
                else
                {
                    return BadRequest(new PasswordResetResponseDto
                    {
                        Success = false,
                        Message = "Failed to update password. Please try again."
                    });
                }
            }
            catch (Exception)
            {
                return BadRequest(new PasswordResetResponseDto
                {
                    Success = false,
                    Message = "An error occurred. Please try again."
                });
            }
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<UserResponseDto>> GetCurrentUser()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User not authenticated");
                }

                var user = await _userService.GetUserByIdAsync(int.Parse(userId));
                return Ok(user);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}