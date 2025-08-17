using Microsoft.AspNetCore.Mvc;
using NetflixClone.DTOs;
using NetflixClone.Interfaces;

namespace NetflixClone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TwoFactorAuthController : ControllerBase
    {
        private readonly ITwoFactorAuthService _twoFactorAuthService;
        private readonly ILogger<TwoFactorAuthController> _logger;

        public TwoFactorAuthController(ITwoFactorAuthService twoFactorAuthService, ILogger<TwoFactorAuthController> logger)
        {
            _twoFactorAuthService = twoFactorAuthService;
            _logger = logger;
        }

        [HttpPost("enable")]
        public async Task<ActionResult<TwoFactorAuthResponseDto>> EnableTwoFactor([FromBody] EnableTwoFactorDto dto)
        {
            try
            {
                var result = await _twoFactorAuthService.EnableTwoFactorAsync(dto.Email);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new TwoFactorAuthResponseDto
                {
                    Success = false,
                    Message = "An error occurred while enabling two-factor authentication"
                });
            }
        }

        [HttpPost("disable")]
        public async Task<ActionResult<TwoFactorAuthResponseDto>> DisableTwoFactor([FromBody] DisableTwoFactorDto dto)
        {
            try
            {
                var result = await _twoFactorAuthService.DisableTwoFactorAsync(dto.Email, dto.Password);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new TwoFactorAuthResponseDto
                {
                    Success = false,
                    Message = "An error occurred while disabling two-factor authentication"
                });
            }
        }

        [HttpGet("status/{email}")]
        public async Task<ActionResult<TwoFactorAuthResponseDto>> GetTwoFactorStatus(string email)
        {
            try
            {
                var result = await _twoFactorAuthService.GetTwoFactorStatusAsync(email);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new TwoFactorAuthResponseDto
                {
                    Success = false,
                    Message = "An error occurred while getting two-factor authentication status"
                });
            }
        }

        [HttpPost("verify")]
        public async Task<ActionResult<TwoFactorVerificationResponseDto>> VerifyTwoFactorCode([FromBody] VerifyTwoFactorDto dto)
        {
            try
            {
                var result = await _twoFactorAuthService.VerifyTwoFactorCodeAsync(dto.Email, dto.Code);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new TwoFactorVerificationResponseDto
                {
                    Success = false,
                    Message = "An error occurred while verifying the code"
                });
            }
        }

        [HttpPost("resend-code")]
        public async Task<ActionResult<TwoFactorAuthResponseDto>> ResendVerificationCode([FromBody] EnableTwoFactorDto dto)
        {
            try
            {
                var codeSent = await _twoFactorAuthService.SendVerificationCodeAsync(dto.Email);
                if (codeSent)
                {
                    return Ok(new TwoFactorAuthResponseDto
                    {
                        Success = true,
                        Message = "Verification code has been resent to your email",
                        Email = dto.Email
                    });
                }
                else
                {
                    return BadRequest(new TwoFactorAuthResponseDto
                    {
                        Success = false,
                        Message = "Failed to send verification code. Please try again."
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new TwoFactorAuthResponseDto
                {
                    Success = false,
                    Message = "An error occurred while resending the verification code"
                });
            }
        }
    }
}


