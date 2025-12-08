using System.ComponentModel.DataAnnotations;

namespace BonyankopAPI.Models
{
    public class RefreshToken
    {
        [Key]
        public Guid RefreshTokenId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public string Token { get; set; } = string.Empty;

        [Required]
        public DateTime ExpiresAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsRevoked { get; set; } = false;

        public DateTime? RevokedAt { get; set; }

        public string? ReplacedByToken { get; set; }

        // Navigation properties
        public User User { get; set; } = null!;
    }
}
