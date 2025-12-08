using System.ComponentModel.DataAnnotations;
using BonyankopAPI.Models;

namespace BonyankopAPI.DTOs;

public class CreateProviderProfileDto
{
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
}

public class UpdateProviderProfileDto
{
    [MaxLength(200)]
    public string? BusinessName { get; set; }

    [MaxLength(2000)]
    public string? Description { get; set; }

    public List<string>? ServicesOffered { get; set; }

    public List<string>? Certifications { get; set; }

    public List<string>? CoverageAreas { get; set; }

    [MaxLength(100)]
    public string? LicenseNumber { get; set; }

    public int? YearsOfExperience { get; set; }
}

public class ProviderProfileResponseDto
{
    public Guid ProviderId { get; set; }
    public Guid UserId { get; set; }
    public ProviderType ProviderType { get; set; }
    public string BusinessName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<string> ServicesOffered { get; set; } = new();
    public List<string> Certifications { get; set; } = new();
    public List<string> CoverageAreas { get; set; } = new();
    public string? LicenseNumber { get; set; }
    public int? YearsOfExperience { get; set; }
    public decimal AverageRating { get; set; }
    public int TotalProjects { get; set; }
    public int TotalRatings { get; set; }
    public decimal CompletionRate { get; set; }
    public decimal? ResponseTimeHours { get; set; }
    public bool IsVerified { get; set; }
    public bool IsFeatured { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // User information
    public string UserEmail { get; set; } = string.Empty;
    public string UserFullName { get; set; } = string.Empty;
    public string? UserPhoneNumber { get; set; }
    public string? UserProfilePictureUrl { get; set; }
}

public class SearchProviderDto
{
    public string? SearchTerm { get; set; }
    public ProviderType? ProviderType { get; set; }
    public string? CoverageArea { get; set; }
}
