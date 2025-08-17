using System;

namespace NetflixClone.Models
{
    public class PasswordReset : BaseEntity
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string ResetCode { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; } = false;
        public DateTime? UsedAt { get; set; }
        
        public User User { get; set; } = null!;
    }
}
