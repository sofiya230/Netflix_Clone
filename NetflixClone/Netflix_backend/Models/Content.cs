using System;
using System.Collections.Generic;

namespace NetflixClone.Models
{
    public class Content : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int ReleaseYear { get; set; }
        public string MaturityRating { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public int? TotalSeasons { get; set; }
        public string ThumbnailUrl { get; set; } = string.Empty;
        public string CoverImageUrl { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public string Director { get; set; } = string.Empty;
        public string Cast { get; set; } = string.Empty;
        
        public List<Episode> Episodes { get; set; } = new List<Episode>();
        public List<WatchHistory> WatchHistory { get; set; } = new List<WatchHistory>();
        public List<MyList> MyList { get; set; } = new List<MyList>();
    }
}
