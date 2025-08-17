using System.ComponentModel.DataAnnotations;

namespace NetflixClone.DTOs
{
    public class UserUpdateDto
    {
        [StringLength(100, MinimumLength = 2)]
        public string? FirstName { get; set; }

        [StringLength(100, MinimumLength = 2)]
        public string? LastName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string? SubscriptionPlan { get; set; }
        
        public string? ProfilePictureUrl { get; set; }
    }
}
