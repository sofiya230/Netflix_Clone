using System;
using System.ComponentModel.DataAnnotations;

namespace NetflixClone.DTOs
{
    public class MyListCreateDto
    {
        [Required(ErrorMessage = "Profile ID is required")]
        public int ProfileId { get; set; }

        [Required(ErrorMessage = "Content ID is required")]
        public int ContentId { get; set; }
    }

    public class MyListResponseDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProfileId { get; set; }
        public int ContentId { get; set; }
        public DateTime AddedAt { get; set; }
        public ContentSummaryDto Content { get; set; } = new ContentSummaryDto();
    }
}
