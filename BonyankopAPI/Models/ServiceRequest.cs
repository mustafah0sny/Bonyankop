using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BonyankopAPI.Models;

public class ServiceRequest
{
    [Key]
    public Guid RequestId { get; set; }

    public Guid? DiagnosticId { get; set; }

    [Required]
    public Guid CitizenId { get; set; }

    [Required]
    [MaxLength(200)]
    public string ProblemTitle { get; set; } = string.Empty;

    [Required]
    [MaxLength(2000)]
    public string ProblemDescription { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string ProblemCategory { get; set; } = string.Empty;

    public List<string> AdditionalImages { get; set; } = new();

    [MaxLength(50)]
    public string? PreferredProviderType { get; set; }

    public DateTime? PreferredServiceDate { get; set; }

    [MaxLength(100)]
    public string? PropertyType { get; set; }

    [MaxLength(300)]
    public string? PropertyAddress { get; set; }

    [MaxLength(20)]
    public string? ContactPhone { get; set; }

    [Required]
    public RequestStatus Status { get; set; } = RequestStatus.OPEN;

    public Guid? SelectedQuoteId { get; set; }

    public int QuotesCount { get; set; } = 0;

    public int ViewsCount { get; set; } = 0;

    public DateTime? ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey(nameof(CitizenId))]
    public virtual User Citizen { get; set; } = null!;

    [ForeignKey(nameof(DiagnosticId))]
    public virtual Diagnostic? Diagnostic { get; set; }
}
