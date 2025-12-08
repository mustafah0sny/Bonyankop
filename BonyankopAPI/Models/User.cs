using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace BonyankopAPI.Models
{
    public class User : IdentityUser<Guid>
    {
        [Required]
        public UserRole Role { get; set; } = UserRole.CITIZEN;

        [Required]
        [StringLength(200)]
        public string FullName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? ProfilePictureUrl { get; set; }

        public bool? IsVerified { get; set; } = false;

        public bool? IsActive { get; set; } = true;

        public DateTime? LastLoginAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
