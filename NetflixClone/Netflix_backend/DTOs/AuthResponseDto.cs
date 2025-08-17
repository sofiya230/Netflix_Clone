namespace NetflixClone.DTOs
{
    public class AuthResponseDto
    {
        public bool IsSuccessful { get; set; }
        public string Message { get; set; }
        public string? Token { get; set; }
        public UserResponseDto? User { get; set; }
        public string? ErrorMessage { get; set; }
        public string? Role { get; set; }
        public bool RequiresTwoFactor { get; set; } = false;
        public string? Email { get; set; }
    }
}
