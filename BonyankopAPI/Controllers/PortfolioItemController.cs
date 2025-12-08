using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BonyankopAPI.DTOs;
using BonyankopAPI.Models;
using BonyankopAPI.Repositories;
using System.Security.Claims;

namespace BonyankopAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PortfolioItemController : ControllerBase
{
    private readonly IPortfolioItemRepository _portfolioItemRepository;
    private readonly IProviderProfileRepository _providerProfileRepository;

    public PortfolioItemController(
        IPortfolioItemRepository portfolioItemRepository,
        IProviderProfileRepository providerProfileRepository)
    {
        _portfolioItemRepository = portfolioItemRepository;
        _providerProfileRepository = providerProfileRepository;
    }

    /// <summary>
    /// Create a new portfolio item
    /// </summary>
    [HttpPost]
    //[Authorize(Roles = "COMPANY,ENGINEER")]
    public async Task<IActionResult> CreatePortfolioItem([FromBody] CreatePortfolioItemDto dto)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "Invalid user token" });
        }

        // Get provider profile
        var providerProfile = await _providerProfileRepository.GetByUserIdAsync(userId);
        if (providerProfile == null)
        {
            return NotFound(new { message = "Provider profile not found. Please create a provider profile first." });
        }

        // Get next display order
        var maxOrder = await _portfolioItemRepository.GetMaxDisplayOrderAsync(providerProfile.ProviderId);

        var portfolioItem = new PortfolioItem
        {
            PortfolioId = Guid.NewGuid(),
            ProviderId = providerProfile.ProviderId,
            Title = dto.Title,
            Description = dto.Description,
            ProjectType = dto.ProjectType,
            Images = dto.Images,
            ProjectDate = dto.ProjectDate,
            Location = dto.Location,
            DisplayOrder = maxOrder + 1,
            IsFeatured = dto.IsFeatured,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _portfolioItemRepository.AddAsync(portfolioItem);
        await _portfolioItemRepository.SaveChangesAsync();

        var response = MapToResponseDto(portfolioItem);
        return CreatedAtAction(nameof(GetPortfolioItem), new { portfolioId = portfolioItem.PortfolioId }, response);
    }

    /// <summary>
    /// Get portfolio item by ID
    /// </summary>
    [HttpGet("{portfolioId}")]
    public async Task<IActionResult> GetPortfolioItem(Guid portfolioId)
    {
        var portfolioItem = await _portfolioItemRepository.GetByIdAsync(portfolioId);
        if (portfolioItem == null)
        {
            return NotFound(new { message = "Portfolio item not found" });
        }

        var response = MapToResponseDto(portfolioItem);
        return Ok(response);
    }

    /// <summary>
    /// Get all portfolio items for a provider
    /// </summary>
    [HttpGet("provider/{providerId}")]
    public async Task<IActionResult> GetProviderPortfolio(Guid providerId)
    {
        var items = await _portfolioItemRepository.GetByProviderIdAsync(providerId);
        var responses = items.Select(MapToResponseDto).ToList();
        return Ok(responses);
    }

    /// <summary>
    /// Get featured portfolio items for a provider
    /// </summary>
    [HttpGet("provider/{providerId}/featured")]
    public async Task<IActionResult> GetProviderFeaturedPortfolio(Guid providerId)
    {
        var items = await _portfolioItemRepository.GetFeaturedByProviderIdAsync(providerId);
        var responses = items.Select(MapToResponseDto).ToList();
        return Ok(responses);
    }

    /// <summary>
    /// Get current user's portfolio items
    /// </summary>
    [HttpGet("my-portfolio")]
    //[Authorize(Roles = "COMPANY,ENGINEER")]
    public async Task<IActionResult> GetMyPortfolio()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "Invalid user token" });
        }

        var providerProfile = await _providerProfileRepository.GetByUserIdAsync(userId);
        if (providerProfile == null)
        {
            return NotFound(new { message = "Provider profile not found" });
        }

        var items = await _portfolioItemRepository.GetByProviderIdAsync(providerProfile.ProviderId);
        var responses = items.Select(MapToResponseDto).ToList();
        return Ok(responses);
    }

    /// <summary>
    /// Update portfolio item
    /// </summary>
    [HttpPut("{portfolioId}")]
    //[Authorize(Roles = "COMPANY,ENGINEER")]
    public async Task<IActionResult> UpdatePortfolioItem(Guid portfolioId, [FromBody] UpdatePortfolioItemDto dto)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "Invalid user token" });
        }

        var providerProfile = await _providerProfileRepository.GetByUserIdAsync(userId);
        if (providerProfile == null)
        {
            return NotFound(new { message = "Provider profile not found" });
        }

        var portfolioItem = await _portfolioItemRepository.GetByIdAndProviderIdAsync(portfolioId, providerProfile.ProviderId);
        if (portfolioItem == null)
        {
            return NotFound(new { message = "Portfolio item not found or you don't have permission to update it" });
        }

        // Update only provided fields
        if (!string.IsNullOrWhiteSpace(dto.Title))
            portfolioItem.Title = dto.Title;

        if (dto.Description != null)
            portfolioItem.Description = dto.Description;

        if (dto.ProjectType != null)
            portfolioItem.ProjectType = dto.ProjectType;

        if (dto.Images != null)
            portfolioItem.Images = dto.Images;

        if (dto.ProjectDate.HasValue)
            portfolioItem.ProjectDate = dto.ProjectDate;

        if (dto.Location != null)
            portfolioItem.Location = dto.Location;

        if (dto.IsFeatured.HasValue)
            portfolioItem.IsFeatured = dto.IsFeatured.Value;

        portfolioItem.UpdatedAt = DateTime.UtcNow;

        _portfolioItemRepository.Update(portfolioItem);
        await _portfolioItemRepository.SaveChangesAsync();

        var response = MapToResponseDto(portfolioItem);
        return Ok(response);
    }

    /// <summary>
    /// Reorder portfolio item
    /// </summary>
    [HttpPatch("{portfolioId}/reorder")]
    //[Authorize(Roles = "COMPANY,ENGINEER")]
    public async Task<IActionResult> ReorderPortfolioItem(Guid portfolioId, [FromBody] ReorderPortfolioItemDto dto)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "Invalid user token" });
        }

        var providerProfile = await _providerProfileRepository.GetByUserIdAsync(userId);
        if (providerProfile == null)
        {
            return NotFound(new { message = "Provider profile not found" });
        }

        var portfolioItem = await _portfolioItemRepository.GetByIdAndProviderIdAsync(portfolioId, providerProfile.ProviderId);
        if (portfolioItem == null)
        {
            return NotFound(new { message = "Portfolio item not found or you don't have permission to reorder it" });
        }

        if (dto.NewOrder < 0)
        {
            return BadRequest(new { message = "Display order cannot be negative" });
        }

        await _portfolioItemRepository.ReorderAsync(providerProfile.ProviderId, portfolioId, dto.NewOrder);

        return Ok(new { message = "Portfolio item reordered successfully" });
    }

    /// <summary>
    /// Delete portfolio item
    /// </summary>
    [HttpDelete("{portfolioId}")]
    //[Authorize(Roles = "COMPANY,ENGINEER")]
    public async Task<IActionResult> DeletePortfolioItem(Guid portfolioId)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "Invalid user token" });
        }

        var providerProfile = await _providerProfileRepository.GetByUserIdAsync(userId);
        if (providerProfile == null)
        {
            return NotFound(new { message = "Provider profile not found" });
        }

        var portfolioItem = await _portfolioItemRepository.GetByIdAndProviderIdAsync(portfolioId, providerProfile.ProviderId);
        if (portfolioItem == null)
        {
            return NotFound(new { message = "Portfolio item not found or you don't have permission to delete it" });
        }

        _portfolioItemRepository.Remove(portfolioItem);
        await _portfolioItemRepository.SaveChangesAsync();

        return Ok(new { message = "Portfolio item deleted successfully" });
    }

    private static PortfolioItemResponseDto MapToResponseDto(PortfolioItem item)
    {
        return new PortfolioItemResponseDto
        {
            PortfolioId = item.PortfolioId,
            ProviderId = item.ProviderId,
            Title = item.Title,
            Description = item.Description,
            ProjectType = item.ProjectType,
            Images = item.Images,
            ProjectDate = item.ProjectDate,
            Location = item.Location,
            DisplayOrder = item.DisplayOrder,
            IsFeatured = item.IsFeatured,
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt
        };
    }
}
