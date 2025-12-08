using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BonyankopAPI.Models;

public class Rating
{
    [Key]
    public Guid RatingId { get; set; }

    [Required]
    public Guid ProjectId { get; set; }

    [Required]
    public Guid CitizenId { get; set; }

    [Required]
    public Guid ProviderId { get; set; }

    [Required]
    [Range(1, 5)]
    public int OverallRating { get; set; }

    [Required]
    [Range(1, 5)]
    public int QualityRating { get; set; }

    [Required]
    [Range(1, 5)]
    public int TimelinessRating { get; set; }

    [Required]
    [Range(1, 5)]
    public int ProfessionalismRating { get; set; }

    [Required]
    [Range(1, 5)]
    public int ValueRating { get; set; }

    [Required]
    [Range(1, 5)]
    public int CommunicationRating { get; set; }

    [MaxLength(200)]
    public string? ReviewTitle { get; set; }

    [MaxLength(2000)]
    public string? ReviewText { get; set; }

    public bool? WouldRecommend { get; set; }

    [MaxLength(2000)]
    public string? ResponseFromProvider { get; set; }

    public DateTime? ResponseAt { get; set; }

    public bool IsVerified { get; set; } = false;

    public bool IsFeatured { get; set; } = false;

    public int HelpfulCount { get; set; } = 0;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey(nameof(ProjectId))]
    public virtual Project Project { get; set; } = null!;

    [ForeignKey(nameof(CitizenId))]
    public virtual User Citizen { get; set; } = null!;

    [ForeignKey(nameof(ProviderId))]
    public virtual ProviderProfile Provider { get; set; } = null!;

    // Business method
    public decimal CalculateAverageRating()
    {
        return (QualityRating + TimelinessRating + ProfessionalismRating + ValueRating + CommunicationRating) / 5.0m;
    }
}
