using System;

namespace NetflixClone.Models
{
    public class Profile : BaseEntity
    {
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public bool IsKidsProfile { get; set; } = false;
        public string MaturityLevel { get; set; } = "All Ages";
        public string Language { get; set; } = "English";
        
        public User User { get; set; } = null!;
        public List<MyList> MyLists { get; set; } = new List<MyList>();
        public List<WatchHistory> WatchHistories { get; set; } = new List<WatchHistory>();
    }
}
