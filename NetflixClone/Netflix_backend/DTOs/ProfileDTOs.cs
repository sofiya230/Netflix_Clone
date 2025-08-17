using System.ComponentModel.DataAnnotations;

namespace NetflixClone.DTOs
{
    public class ProfileCreateDto
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
        public string Name { get; set; } = string.Empty;

        public string AvatarUrl { get; set; } = string.Empty;
        public bool IsKidsProfile { get; set; } = false;
        public string Language { get; set; } = "en-US";
        public string MaturityLevel { get; set; } = "Adult";
    }

    public class ProfileUpdateDto
    {
        [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
        public string? Name { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Language { get; set; }
        public string? MaturityLevel { get; set; }
    }

    public class ProfileResponseDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public bool IsKidsProfile { get; set; }
        public string Language { get; set; } = string.Empty;
        public string MaturityLevel { get; set; } = string.Empty;
    }
}
