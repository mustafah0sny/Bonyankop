using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BonyankopAPI.DTOs;
using BonyankopAPI.Models;
using BonyankopAPI.Repositories;
using BonyankopAPI.Interfaces;
using System.Security.Claims;

namespace BonyankopAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
//[Authorize(Roles = "COMPANY,ENGINEER")]

public class ProviderProfileController : ControllerBase
{
    private readonly IProviderProfileRepository _providerProfileRepository;
    private readonly IUserRepository _userRepository;

    public ProviderProfileController(
        IProviderProfileRepository providerProfileRepository,
        IUserRepository userRepository)
    {
        _providerProfileRepository = providerProfileRepository;
        _userRepository = userRepository;
    }

    /// <summary>
    /// Create a new provider profile
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateProfile([FromBody] CreateProviderProfileDto dto)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "Invalid user token" });
        }

        // Check if user exists
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        // Check if user already has a provider profile
        if (await _providerProfileRepository.ExistsByUserIdAsync(userId))
        {
            return Conflict(new { message = "Provider profile already exists for this user" });
        }

        // Check if user role matches provider type
        var requiredRole = dto.ProviderType == ProviderType.COMPANY ? UserRole.COMPANY : UserRole.ENGINEER;
        if (user.Role != requiredRole)
        {
            return BadRequest(new { message = $"User role must be {requiredRole} to create a {dto.ProviderType} profile" });
        }

        var providerProfile = new ProviderProfile
        {
            ProviderId = Guid.NewGuid(),
            UserId = userId,
            ProviderType = dto.ProviderType,
            BusinessName = dto.BusinessName,
            Description = dto.Description,
            ServicesOffered = dto.ServicesOffered,
            Certifications = dto.Certifications,
            CoverageAreas = dto.CoverageAreas,
            LicenseNumber = dto.LicenseNumber,
            YearsOfExperience = dto.YearsOfExperience,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _providerProfileRepository.AddAsync(providerProfile);
        await _providerProfileRepository.SaveChangesAsync();

        var response = MapToResponseDto(providerProfile, user);
        return CreatedAtAction(nameof(GetProfile), new { providerId = providerProfile.ProviderId }, response);
    }

    /// <summary>
    /// Get provider profile by ID
    /// </summary>
    [HttpGet("{providerId}")]
    public async Task<IActionResult> GetProfile(Guid providerId)
    {
        var profile = await _providerProfileRepository.GetByIdAsync(providerId);
        if (profile == null)
        {
            return NotFound(new { message = "Provider profile not found" });
        }

        var user = await _userRepository.GetByIdAsync(profile.UserId);
        if (user == null)
        {
            return NotFound(new { message = "Associated user not found" });
        }

        var response = MapToResponseDto(profile, user);
        return Ok(response);
    }

    /// <summary>
    /// Get current user's provider profile
    /// </summary>
    [HttpGet("my-profile")]
    //[Authorize(Roles = "COMPANY,ENGINEER")]
    public async Task<IActionResult> GetMyProfile()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "Invalid user token" });
        }

        var profile = await _providerProfileRepository.GetByUserIdAsync(userId);
        if (profile == null)
        {
            return NotFound(new { message = "Provider profile not found" });
        }

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        var response = MapToResponseDto(profile, user);
        return Ok(response);
    }

    /// <summary>
    /// Update provider profile
    /// </summary>
    [HttpPut]
    //[Authorize(Roles = "COMPANY,ENGINEER")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProviderProfileDto dto)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "Invalid user token" });
        }

        var profile = await _providerProfileRepository.GetByUserIdAsync(userId);
        if (profile == null)
        {
            return NotFound(new { message = "Provider profile not found" });
        }

        // Update only provided fields
        if (!string.IsNullOrWhiteSpace(dto.BusinessName))
            profile.BusinessName = dto.BusinessName;
        
        if (dto.Description != null)
            profile.Description = dto.Description;
        
        if (dto.ServicesOffered != null)
            profile.ServicesOffered = dto.ServicesOffered;
        
        if (dto.Certifications != null)
            profile.Certifications = dto.Certifications;
        
        if (dto.CoverageAreas != null)
            profile.CoverageAreas = dto.CoverageAreas;
        
        if (dto.LicenseNumber != null)
            profile.LicenseNumber = dto.LicenseNumber;
        
        if (dto.YearsOfExperience.HasValue)
            profile.YearsOfExperience = dto.YearsOfExperience;

        profile.UpdatedAt = DateTime.UtcNow;

        _providerProfileRepository.Update(profile);
        await _providerProfileRepository.SaveChangesAsync();

        var user = await _userRepository.GetByIdAsync(userId);
        var response = MapToResponseDto(profile, user!);
        return Ok(response);
    }

    /// <summary>
    /// Search providers
    /// </summary>
    [HttpGet("search")]
    public async Task<IActionResult> SearchProviders([FromQuery] SearchProviderDto dto)
    {
        var providers = await _providerProfileRepository.SearchProvidersAsync(
            dto.SearchTerm,
            dto.ProviderType,
            dto.CoverageArea
        );

        var responses = new List<ProviderProfileResponseDto>();
        foreach (var provider in providers)
        {
            var user = await _userRepository.GetByIdAsync(provider.UserId);
            if (user != null)
            {
                responses.Add(MapToResponseDto(provider, user));
            }
        }

        return Ok(responses);
    }

    /// <summary>
    /// Get all verified providers
    /// </summary>
    [HttpGet("verified")]
    public async Task<IActionResult> GetVerifiedProviders()
    {
        var providers = await _providerProfileRepository.GetVerifiedProvidersAsync();

        var responses = new List<ProviderProfileResponseDto>();
        foreach (var provider in providers)
        {
            var user = await _userRepository.GetByIdAsync(provider.UserId);
            if (user != null)
            {
                responses.Add(MapToResponseDto(provider, user));
            }
        }

        return Ok(responses);
    }

    /// <summary>
    /// Get featured providers
    /// </summary>
    [HttpGet("featured")]
    public async Task<IActionResult> GetFeaturedProviders()
    {
        var providers = await _providerProfileRepository.GetFeaturedProvidersAsync();

        var responses = new List<ProviderProfileResponseDto>();
        foreach (var provider in providers)
        {
            var user = await _userRepository.GetByIdAsync(provider.UserId);
            if (user != null)
            {
                responses.Add(MapToResponseDto(provider, user));
            }
        }

        return Ok(responses);
    }

    /// <summary>
    /// Get providers by type
    /// </summary>
    [HttpGet("by-type/{providerType}")]
    public async Task<IActionResult> GetByProviderType(ProviderType providerType)
    {
        var providers = await _providerProfileRepository.GetByProviderTypeAsync(providerType);

        var responses = new List<ProviderProfileResponseDto>();
        foreach (var provider in providers)
        {
            var user = await _userRepository.GetByIdAsync(provider.UserId);
            if (user != null)
            {
                responses.Add(MapToResponseDto(provider, user));
            }
        }

        return Ok(responses);
    }

    private static ProviderProfileResponseDto MapToResponseDto(ProviderProfile profile, User user)
    {
        return new ProviderProfileResponseDto
        {
            ProviderId = profile.ProviderId,
            UserId = profile.UserId,
            ProviderType = profile.ProviderType,
            BusinessName = profile.BusinessName,
            Description = profile.Description,
            ServicesOffered = profile.ServicesOffered,
            Certifications = profile.Certifications,
            CoverageAreas = profile.CoverageAreas,
            LicenseNumber = profile.LicenseNumber,
            YearsOfExperience = profile.YearsOfExperience,
            AverageRating = profile.AverageRating,
            TotalProjects = profile.TotalProjects,
            TotalRatings = profile.TotalRatings,
            CompletionRate = profile.CompletionRate,
            ResponseTimeHours = profile.ResponseTimeHours,
            IsVerified = profile.IsVerified,
            IsFeatured = profile.IsFeatured,
            CreatedAt = profile.CreatedAt,
            UpdatedAt = profile.UpdatedAt,
            UserEmail = user.Email!,
            UserFullName = user.FullName,
            UserPhoneNumber = user.PhoneNumber,
            UserProfilePictureUrl = user.ProfilePictureUrl
        };
    }
}
