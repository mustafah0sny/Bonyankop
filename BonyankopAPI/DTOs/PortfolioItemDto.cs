using System.ComponentModel.DataAnnotations;

namespace BonyankopAPI.DTOs;

public class CreatePortfolioItemDto
{
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

    public bool IsFeatured { get; set; } = false;
}

public class UpdatePortfolioItemDto
{
    [MaxLength(200)]
    public string? Title { get; set; }

    [MaxLength(2000)]
    public string? Description { get; set; }

    [MaxLength(100)]
    public string? ProjectType { get; set; }

    public List<string>? Images { get; set; }

    public DateTime? ProjectDate { get; set; }

    [MaxLength(200)]
    public string? Location { get; set; }

    public bool? IsFeatured { get; set; }
}

public class ReorderPortfolioItemDto
{
    [Required]
    public int NewOrder { get; set; }
}

public class PortfolioItemResponseDto
{
    public Guid PortfolioId { get; set; }
    public Guid ProviderId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ProjectType { get; set; }
    public List<string> Images { get; set; } = new();
    public DateTime? ProjectDate { get; set; }
    public string? Location { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsFeatured { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
