using System.ComponentModel.DataAnnotations;

namespace BonyankopAPI.Models
{
    public class RolePermission
    {
        [Key]
        public Guid RolePermissionId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid RoleId { get; set; }

        [Required]
        public Guid PermissionId { get; set; }

        public DateTime GrantedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Role Role { get; set; } = null!;
        public Permission Permission { get; set; } = null!;
    }
}
