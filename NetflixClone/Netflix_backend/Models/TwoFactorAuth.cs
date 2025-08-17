using System.ComponentModel.DataAnnotations;

namespace NetflixClone.Models
{
    public class TwoFactorAuth
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int UserId { get; set; }
        
        [Required]
        public bool IsEnabled { get; set; } = false;
        
        [Required]
        public string Email { get; set; }
        
        [Required]
        public string VerificationCode { get; set; }
        
        [Required]
        public DateTime CodeGeneratedAt { get; set; }
        
        [Required]
        public DateTime CodeExpiresAt { get; set; }
        
        [Required]
        public bool IsVerified { get; set; } = false;
        
        public DateTime? LastUsedAt { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        public virtual User User { get; set; }
    }
}


