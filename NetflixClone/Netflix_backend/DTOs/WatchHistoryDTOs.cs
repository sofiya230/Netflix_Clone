using System;
using System.ComponentModel.DataAnnotations;

namespace NetflixClone.DTOs
{
    public class WatchHistoryCreateDto
    {
        [Required(ErrorMessage = "Profile ID is required")]
        public int ProfileId { get; set; }

        [Required(ErrorMessage = "Content ID is required")]
        public int ContentId { get; set; }

        public int? EpisodeId { get; set; }

        [Required(ErrorMessage = "Watched percentage is required")]
        [Range(0, 100, ErrorMessage = "Watched percentage must be between 0 and 100")]
        public double WatchedPercentage { get; set; }

        [Required(ErrorMessage = "Watched duration is required")]
        public TimeSpan WatchedDuration { get; set; }

        public bool IsCompleted { get; set; } = false;
    }

    public class WatchHistoryUpdateDto
    {
        [Range(0, 100, ErrorMessage = "Watched percentage must be between 0 and 100")]
        public double WatchedPercentage { get; set; }
        public TimeSpan WatchedDuration { get; set; }
        public bool IsCompleted { get; set; }
    }

    public class WatchHistoryResponseDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProfileId { get; set; }
        public int ContentId { get; set; }
        public int? EpisodeId { get; set; }
        public double WatchedPercentage { get; set; }
        public DateTime LastWatched { get; set; }
        public TimeSpan WatchedDuration { get; set; }
        public bool IsCompleted { get; set; }
        public ContentSummaryDto Content { get; set; } = new ContentSummaryDto();
        public EpisodeResponseDto? Episode { get; set; }
    }

    public class ContinueWatchingDto
    {
        public int ContentId { get; set; }
        public int? EpisodeId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public double WatchedPercentage { get; set; }
        public DateTime LastWatched { get; set; }
    }
}
