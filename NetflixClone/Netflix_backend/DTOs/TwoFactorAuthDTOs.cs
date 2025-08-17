namespace NetflixClone.DTOs
{
    public class EnableTwoFactorDto
    {
        public string Email { get; set; }
    }

    public class DisableTwoFactorDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class VerifyTwoFactorDto
    {
        public string Email { get; set; }
        public string Code { get; set; }
    }

    public class TwoFactorAuthResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public bool IsEnabled { get; set; }
        public string Email { get; set; }
    }

    public class TwoFactorVerificationResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public bool IsVerified { get; set; }
        public string Token { get; set; }
        public UserResponseDto User { get; set; }
        public string Role { get; set; }
    }
}

