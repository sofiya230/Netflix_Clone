using System.Text.Json.Serialization;

namespace NetflixClone.DTOs
{
    public class PasswordResetResponseDto
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }
        
        [JsonPropertyName("message")]
        public string Message { get; set; }
        
        [JsonPropertyName("resetCode")]
        public string? ResetCode { get; set; }
        
        [JsonPropertyName("email")]
        public string Email { get; set; }
    }
}
