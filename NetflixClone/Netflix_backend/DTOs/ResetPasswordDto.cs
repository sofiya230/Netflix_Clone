using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace NetflixClone.DTOs
{
    public class ResetPasswordDto
    {
        [Required]
        [EmailAddress]
        [JsonPropertyName("email")]
        public string Email { get; set; }
        
        [Required]
        [StringLength(6, MinimumLength = 6)]
        [JsonPropertyName("resetCode")]
        public string ResetCode { get; set; }
        
        [Required]
        [StringLength(100, MinimumLength = 6)]
        [JsonPropertyName("newPassword")]
        public string NewPassword { get; set; }
        
        [Required]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        [JsonPropertyName("confirmPassword")]
        public string ConfirmPassword { get; set; }
    }
}


