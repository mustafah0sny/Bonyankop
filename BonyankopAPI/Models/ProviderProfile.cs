using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BonyankopAPI.Models;

public class ProviderProfile
{
    [Key]
    public Guid ProviderId { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    public ProviderType ProviderType { get; set; }

    [Required]
    [MaxLength(200)]
    public string BusinessName { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    public List<string> ServicesOffered { get; set; } = new();

    public List<string> Certifications { get; set; } = new();

    public List<string> CoverageAreas { get; set; } = new();

    [MaxLength(100)]
    public string? LicenseNumber { get; set; }

    public int? YearsOfExperience { get; set; }

    [Column(TypeName = "decimal(3,2)")]
    public decimal AverageRating { get; set; } = 0.0m;

    public int TotalProjects { get; set; } = 0;

    public int TotalRatings { get; set; } = 0;

    [Column(TypeName = "decimal(5,2)")]
    public decimal CompletionRate { get; set; } = 0.0m;

    [Column(TypeName = "decimal(8,2)")]
    public decimal? ResponseTimeHours { get; set; }

    public bool IsVerified { get; set; } = false;

    public bool IsFeatured { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    [ForeignKey(nameof(UserId))]
    public virtual User User { get; set; } = null!;
}
