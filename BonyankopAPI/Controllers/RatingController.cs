using BonyankopAPI.DTOs;
using BonyankopAPI.Interfaces;
using BonyankopAPI.Models;
using BonyankopAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BonyankopAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RatingController : ControllerBase
{
    private readonly IRatingRepository _ratingRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IProviderProfileRepository _providerProfileRepository;
    private readonly IUserRepository _userRepository;

    public RatingController(
        IRatingRepository ratingRepository,
        IProjectRepository projectRepository,
        IProviderProfileRepository providerProfileRepository,
        IUserRepository userRepository)
    {
        _ratingRepository = ratingRepository;
        _projectRepository = projectRepository;
        _providerProfileRepository = providerProfileRepository;
        _userRepository = userRepository;
    }

    [HttpPost]
    [Authorize(Roles = "CITIZEN")]
    public async Task<IActionResult> CreateRating([FromBody] CreateRatingDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        // Get and validate project
        var project = await _projectRepository.GetByIdAsync(dto.ProjectId);
        if (project == null)
        {
            return NotFound(new { message = "Project not found" });
        }

        if (project.CitizenId != Guid.Parse(userId))
        {
            return Forbid();
        }

        if (project.Status != ProjectStatus.COMPLETED)
        {
            return BadRequest(new { message = "Only completed projects can be rated" });
        }

        // Check if rating already exists
        var existingRating = await _ratingRepository.GetByProjectAndCitizenAsync(dto.ProjectId, Guid.Parse(userId));
        if (existingRating != null)
        {
            return BadRequest(new { message = "You have already rated this project" });
        }

        var rating = new Rating
        {
            ProjectId = dto.ProjectId,
            CitizenId = Guid.Parse(userId),
            ProviderId = project.ProviderId,
            OverallRating = dto.OverallRating,
            QualityRating = dto.QualityRating,
            TimelinessRating = dto.TimelinessRating,
            ProfessionalismRating = dto.ProfessionalismRating,
            ValueRating = dto.ValueRating,
            CommunicationRating = dto.CommunicationRating,
            ReviewTitle = dto.ReviewTitle,
            ReviewText = dto.ReviewText,
            WouldRecommend = dto.WouldRecommend,
            IsVerified = true // Auto-verify since they completed a project
        };

        await _ratingRepository.AddAsync(rating);
        await _ratingRepository.SaveChangesAsync();

        // Update provider's average rating
        await UpdateProviderRating(project.ProviderId);

        var response = await MapToResponseDto(rating);
        return CreatedAtAction(nameof(GetRating), new { id = rating.RatingId }, response);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetRating(Guid id)
    {
        var rating = await _ratingRepository.GetByIdAsync(id);
        if (rating == null)
        {
            return NotFound(new { message = "Rating not found" });
        }

        var response = await MapToResponseDto(rating);
        return Ok(response);
    }

    [HttpGet("project/{projectId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetRatingsByProject(Guid projectId)
    {
        var ratings = await _ratingRepository.GetByProjectIdAsync(projectId);
        var response = new List<RatingResponseDto>();

        foreach (var rating in ratings)
        {
            response.Add(await MapToResponseDto(rating));
        }

        return Ok(response);
    }

    [HttpGet("provider/{providerId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetRatingsByProvider(Guid providerId)
    {
        var ratings = await _ratingRepository.GetByProviderIdAsync(providerId);
        var response = new List<RatingResponseDto>();

        foreach (var rating in ratings)
        {
            response.Add(await MapToResponseDto(rating));
        }

        return Ok(response);
    }

    [HttpGet("my-ratings")]
    [Authorize(Roles = "CITIZEN")]
    public async Task<IActionResult> GetMyRatings()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var ratings = await _ratingRepository.GetByCitizenIdAsync(Guid.Parse(userId));
        var response = new List<RatingResponseDto>();

        foreach (var rating in ratings)
        {
            response.Add(await MapToResponseDto(rating));
        }

        return Ok(response);
    }

    [HttpGet("featured")]
    [AllowAnonymous]
    public async Task<IActionResult> GetFeaturedRatings([FromQuery] int count = 10)
    {
        var ratings = await _ratingRepository.GetFeaturedRatingsAsync(count);
        var response = new List<RatingResponseDto>();

        foreach (var rating in ratings)
        {
            response.Add(await MapToResponseDto(rating));
        }

        return Ok(response);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "CITIZEN")]
    public async Task<IActionResult> UpdateRating(Guid id, [FromBody] UpdateRatingDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var rating = await _ratingRepository.GetByIdAsync(id);
        if (rating == null)
        {
            return NotFound(new { message = "Rating not found" });
        }

        if (rating.CitizenId != Guid.Parse(userId))
        {
            return Forbid();
        }

        // Update fields
        if (dto.OverallRating.HasValue) rating.OverallRating = dto.OverallRating.Value;
        if (dto.QualityRating.HasValue) rating.QualityRating = dto.QualityRating.Value;
        if (dto.TimelinessRating.HasValue) rating.TimelinessRating = dto.TimelinessRating.Value;
        if (dto.ProfessionalismRating.HasValue) rating.ProfessionalismRating = dto.ProfessionalismRating.Value;
        if (dto.ValueRating.HasValue) rating.ValueRating = dto.ValueRating.Value;
        if (dto.CommunicationRating.HasValue) rating.CommunicationRating = dto.CommunicationRating.Value;
        if (dto.ReviewTitle != null) rating.ReviewTitle = dto.ReviewTitle;
        if (dto.ReviewText != null) rating.ReviewText = dto.ReviewText;
        if (dto.WouldRecommend.HasValue) rating.WouldRecommend = dto.WouldRecommend;

        rating.UpdatedAt = DateTime.UtcNow;

        _ratingRepository.Update(rating);
        await _ratingRepository.SaveChangesAsync();

        // Update provider's average rating
        await UpdateProviderRating(rating.ProviderId);

        var response = await MapToResponseDto(rating);
        return Ok(response);
    }

    [HttpPost("{id}/response")]
    [Authorize(Roles = "COMPANY,ENGINEER")]
    public async Task<IActionResult> AddProviderResponse(Guid id, [FromBody] string response)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var rating = await _ratingRepository.GetByIdAsync(id);
        if (rating == null)
        {
            return NotFound(new { message = "Rating not found" });
        }

        var providerProfile = await _providerProfileRepository.GetByUserIdAsync(Guid.Parse(userId));
        if (providerProfile == null || providerProfile.ProviderId != rating.ProviderId)
        {
            return Forbid();
        }

        rating.ResponseFromProvider = response;
        rating.ResponseAt = DateTime.UtcNow;
        rating.UpdatedAt = DateTime.UtcNow;

        _ratingRepository.Update(rating);
        await _ratingRepository.SaveChangesAsync();

        return Ok(new { message = "Response added successfully" });
    }

    [HttpPost("{id}/helpful")]
    [AllowAnonymous]
    public async Task<IActionResult> MarkHelpful(Guid id)
    {
        var rating = await _ratingRepository.GetByIdAsync(id);
        if (rating == null)
        {
            return NotFound(new { message = "Rating not found" });
        }

        rating.HelpfulCount++;
        rating.UpdatedAt = DateTime.UtcNow;

        _ratingRepository.Update(rating);
        await _ratingRepository.SaveChangesAsync();

        return Ok(new { message = "Marked as helpful", helpfulCount = rating.HelpfulCount });
    }

    [HttpPost("{id}/verify")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> VerifyRating(Guid id)
    {
        var rating = await _ratingRepository.GetByIdAsync(id);
        if (rating == null)
        {
            return NotFound(new { message = "Rating not found" });
        }

        rating.IsVerified = true;
        rating.UpdatedAt = DateTime.UtcNow;

        _ratingRepository.Update(rating);
        await _ratingRepository.SaveChangesAsync();

        return Ok(new { message = "Rating verified successfully" });
    }

    [HttpPost("{id}/feature")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> FeatureRating(Guid id)
    {
        var rating = await _ratingRepository.GetByIdAsync(id);
        if (rating == null)
        {
            return NotFound(new { message = "Rating not found" });
        }

        rating.IsFeatured = !rating.IsFeatured;
        rating.UpdatedAt = DateTime.UtcNow;

        _ratingRepository.Update(rating);
        await _ratingRepository.SaveChangesAsync();

        return Ok(new { message = rating.IsFeatured ? "Rating featured" : "Rating unfeatured" });
    }

    private async Task UpdateProviderRating(Guid providerId)
    {
        var averageRating = await _ratingRepository.GetProviderAverageRatingAsync(providerId);
        var ratings = await _ratingRepository.GetByProviderIdAsync(providerId);
        
        var provider = await _providerProfileRepository.GetByIdAsync(providerId);
        if (provider != null)
        {
            provider.AverageRating = averageRating;
            provider.TotalRatings = ratings.Count();
            provider.UpdatedAt = DateTime.UtcNow;

            _providerProfileRepository.Update(provider);
            await _providerProfileRepository.SaveChangesAsync();
        }
    }

    private async Task<RatingResponseDto> MapToResponseDto(Rating rating)
    {
        var citizen = await _userRepository.GetByIdAsync(rating.CitizenId);
        var provider = await _providerProfileRepository.GetByIdAsync(rating.ProviderId);
        var providerUser = provider != null ? await _userRepository.GetByIdAsync(provider.UserId) : null;

        return new RatingResponseDto
        {
            RatingId = rating.RatingId,
            ProjectId = rating.ProjectId,
            CitizenId = rating.CitizenId,
            CitizenName = citizen?.FullName ?? "Unknown",
            ProviderId = rating.ProviderId,
            ProviderName = providerUser?.FullName ?? provider?.BusinessName ?? "Unknown",
            OverallRating = rating.OverallRating,
            QualityRating = rating.QualityRating,
            TimelinessRating = rating.TimelinessRating,
            ProfessionalismRating = rating.ProfessionalismRating,
            ValueRating = rating.ValueRating,
            CommunicationRating = rating.CommunicationRating,
            ReviewTitle = rating.ReviewTitle,
            ReviewText = rating.ReviewText,
            WouldRecommend = rating.WouldRecommend,
            ResponseFromProvider = rating.ResponseFromProvider,
            ResponseAt = rating.ResponseAt,
            IsVerified = rating.IsVerified,
            IsFeatured = rating.IsFeatured,
            HelpfulCount = rating.HelpfulCount,
            AverageRating = rating.CalculateAverageRating(),
            CreatedAt = rating.CreatedAt,
            UpdatedAt = rating.UpdatedAt
        };
    }
}
