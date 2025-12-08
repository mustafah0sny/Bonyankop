using System.ComponentModel.DataAnnotations;

namespace BonyankopAPI.Models
{
    public class UserRoleMapping
    {
        [Key]
        public Guid UserRoleMappingId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid RoleId { get; set; }

        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

        public Guid? AssignedBy { get; set; }

        // Navigation properties
        public User User { get; set; } = null!;
        public Role Role { get; set; } = null!;
    }
}
