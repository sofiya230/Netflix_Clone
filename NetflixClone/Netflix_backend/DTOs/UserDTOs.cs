using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NetflixClone.DTOs
{
    public class UserResponseDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string ProfilePictureUrl { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string SubscriptionPlan { get; set; } = string.Empty;
        public List<ProfileResponseDto> Profiles { get; set; } = new List<ProfileResponseDto>();
    }
}
