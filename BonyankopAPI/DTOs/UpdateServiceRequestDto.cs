using System.ComponentModel.DataAnnotations;

namespace BonyankopAPI.DTOs;

public class UpdateServiceRequestDto
{
    [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    public string? ProblemTitle { get; set; }

    [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
    public string? ProblemDescription { get; set; }

    public string? ProblemCategory { get; set; }

    public List<string>? AdditionalImages { get; set; }

    public string? PreferredProviderType { get; set; }

    public DateTime? PreferredServiceDate { get; set; }

    [StringLength(100, ErrorMessage = "Property type cannot exceed 100 characters")]
    public string? PropertyType { get; set; }

    [StringLength(300, ErrorMessage = "Property address cannot exceed 300 characters")]
    public string? PropertyAddress { get; set; }

    [StringLength(20, ErrorMessage = "Contact phone cannot exceed 20 characters")]
    public string? ContactPhone { get; set; }

    public DateTime? ExpiresAt { get; set; }
}
