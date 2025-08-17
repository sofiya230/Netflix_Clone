using System;

namespace NetflixClone.Models
{
    public class WatchHistory : BaseEntity
    {
        public int UserId { get; set; }
        public int ProfileId { get; set; }
        public int ContentId { get; set; }
        public int? EpisodeId { get; set; }
        public double WatchedPercentage { get; set; }
        public TimeSpan WatchedDuration { get; set; }
        public DateTime LastWatched { get; set; } = DateTime.UtcNow;
        public bool IsCompleted { get; set; } = false;
        
        public User User { get; set; } = null!;
        public Profile Profile { get; set; } = null!;
        public Content Content { get; set; } = null!;
        public Episode? Episode { get; set; }
    }
}
