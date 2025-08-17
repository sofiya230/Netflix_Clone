using System.ComponentModel.DataAnnotations;

namespace NetflixClone.DTOs
{
    public class ChangeSubscriptionDto
    {
        [Required]
        public string NewSubscriptionPlan { get; set; }
    }
}
