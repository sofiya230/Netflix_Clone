using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace NetflixClone.DTOs
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        [JsonPropertyName("email")]
        public string Email { get; set; }
    }
}


