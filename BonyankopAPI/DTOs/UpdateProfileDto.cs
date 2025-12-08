using System.ComponentModel.DataAnnotations;

namespace BonyankopAPI.DTOs
{
    public class UpdateProfileDto
    {
        [StringLength(200)]
        public string? FullName { get; set; }

        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        [StringLength(500)]
        public string? ProfilePictureUrl { get; set; }
    }
}
