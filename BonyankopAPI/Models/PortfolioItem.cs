using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BonyankopAPI.Models;

public class PortfolioItem
{
    [Key]
    public Guid PortfolioId { get; set; }

    [Required]
    public Guid ProviderId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    [MaxLength(100)]
    public string? ProjectType { get; set; }

    public List<string> Images { get; set; } = new();

    public DateTime? ProjectDate { get; set; }

    [MaxLength(200)]
    public string? Location { get; set; }

    public int DisplayOrder { get; set; } = 0;

    public bool IsFeatured { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    [ForeignKey(nameof(ProviderId))]
    public virtual ProviderProfile ProviderProfile { get; set; } = null!;
}
