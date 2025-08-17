using System;

namespace NetflixClone.Models
{
    public class MyList : BaseEntity
    {
        public int UserId { get; set; }
        public int ProfileId { get; set; }
        public int ContentId { get; set; }
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
        
        public User User { get; set; } = null!;
        public Profile Profile { get; set; } = null!;
        public Content Content { get; set; } = null!;
    }
}
