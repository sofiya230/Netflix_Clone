using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NetflixClone.DTOs
{
    public class CreateContentDto
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
    }

    public class UpdateContentDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int? ReleaseYear { get; set; }
        public string? MaturityRating { get; set; }
        public string? Duration { get; set; }
        public int? TotalSeasons { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string? CoverImageUrl { get; set; }
        public string? VideoUrl { get; set; }
        public string? ContentType { get; set; }
        public string? Genre { get; set; }
        public string? Director { get; set; }
        public string? Cast { get; set; }
    }

    public class ContentResponseDto
    {
        public int Id { get; set; }
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
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<EpisodeResponseDto> Episodes { get; set; } = new List<EpisodeResponseDto>();
    }

    public class ContentSummaryDto
    {
        public int Id { get; set; }
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
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
