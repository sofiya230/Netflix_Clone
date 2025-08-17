using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace NetflixClone.DTOs
{
    public class VerifyResetCodeDto
    {
        [Required]
        [EmailAddress]
        [JsonPropertyName("email")]
        public string Email { get; set; }
        
        [Required]
        [StringLength(6, MinimumLength = 6)]
        [JsonPropertyName("resetCode")]
        public string ResetCode { get; set; }
    }
}


