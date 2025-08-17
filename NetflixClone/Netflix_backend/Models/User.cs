using System;
using System.Collections.Generic;

namespace NetflixClone.Models
{
    public class User : BaseEntity
    {
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string SubscriptionPlan { get; set; } = "Basic";
        public bool IsEmailVerified { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public string Role { get; set; } = "User";
        public string? ProfilePictureUrl { get; set; }
        public DateTime? LastLoginAt { get; set; }
        
        public List<Profile> Profiles { get; set; } = new List<Profile>();
        public List<PasswordReset> PasswordResets { get; set; } = new List<PasswordReset>();
        public List<TwoFactorAuth> TwoFactorAuths { get; set; } = new List<TwoFactorAuth>();
        public List<MyList> MyLists { get; set; } = new List<MyList>();
        public List<WatchHistory> WatchHistories { get; set; } = new List<WatchHistory>();
    }
}
