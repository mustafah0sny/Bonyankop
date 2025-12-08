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
//[Authorize]
public class ServiceRequestController : ControllerBase
{
    private readonly IServiceRequestRepository _serviceRequestRepository;
    private readonly IUserRepository _userRepository;
    private readonly IDiagnosticRepository _diagnosticRepository;

    public ServiceRequestController(
        IServiceRequestRepository serviceRequestRepository,
        IUserRepository userRepository,
        IDiagnosticRepository diagnosticRepository)
    {
        _serviceRequestRepository = serviceRequestRepository;
        _userRepository = userRepository;
        _diagnosticRepository = diagnosticRepository;
    }

    [HttpPost]
    //[Authorize(Roles = "CITIZEN")]
    public async Task<IActionResult> CreateServiceRequest([FromBody] CreateServiceRequestDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        // Validate diagnostic if provided
        if (dto.DiagnosticId.HasValue)
        {
            var diagnostic = await _diagnosticRepository.GetByIdAsync(dto.DiagnosticId.Value);
            if (diagnostic == null || diagnostic.CitizenId != Guid.Parse(userId))
            {
                return BadRequest(new { message = "Invalid diagnostic ID or you don't own this diagnostic" });
            }
        }

        var serviceRequest = new ServiceRequest
        {
            DiagnosticId = dto.DiagnosticId,
            CitizenId = Guid.Parse(userId),
            ProblemTitle = dto.ProblemTitle,
            ProblemDescription = dto.ProblemDescription,
            ProblemCategory = dto.ProblemCategory,
            AdditionalImages = dto.AdditionalImages,
            PreferredProviderType = dto.PreferredProviderType,
            PreferredServiceDate = dto.PreferredServiceDate,
            PropertyType = dto.PropertyType,
            PropertyAddress = dto.PropertyAddress,
            ContactPhone = dto.ContactPhone,
            ExpiresAt = dto.ExpiresAt
        };

        await _serviceRequestRepository.AddAsync(serviceRequest);
        await _serviceRequestRepository.SaveChangesAsync();

        var user = await _userRepository.GetByIdAsync(Guid.Parse(userId));
        var response = new ServiceRequestResponseDto
        {
            RequestId = serviceRequest.RequestId,
            DiagnosticId = serviceRequest.DiagnosticId,
            CitizenId = serviceRequest.CitizenId,
            CitizenName = user?.FullName ?? "Unknown",
            ProblemTitle = serviceRequest.ProblemTitle,
            ProblemDescription = serviceRequest.ProblemDescription,
            ProblemCategory = serviceRequest.ProblemCategory,
            AdditionalImages = serviceRequest.AdditionalImages,
            PreferredProviderType = serviceRequest.PreferredProviderType,
            PreferredServiceDate = serviceRequest.PreferredServiceDate,
            PropertyType = serviceRequest.PropertyType,PropertyAddress = serviceRequest.PropertyAddress,ContactPhone = serviceRequest.ContactPhone,
            Status = serviceRequest.Status,
            QuotesCount = serviceRequest.QuotesCount,
            ViewsCount = serviceRequest.ViewsCount,
            ExpiresAt = serviceRequest.ExpiresAt,
            CreatedAt = serviceRequest.CreatedAt,
            UpdatedAt = serviceRequest.UpdatedAt
        };

        return CreatedAtAction(nameof(GetServiceRequest), new { id = serviceRequest.RequestId }, response);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetServiceRequest(Guid id)
    {
        var serviceRequest = await _serviceRequestRepository.GetByIdAsync(id);
        if (serviceRequest == null)
        {
            return NotFound(new { message = "Service request not found" });
        }

        // Increment views count
        await _serviceRequestRepository.IncrementViewsAsync(id);

        var user = await _userRepository.GetByIdAsync(serviceRequest.CitizenId);
        var response = new ServiceRequestResponseDto
        {
            RequestId = serviceRequest.RequestId,
            DiagnosticId = serviceRequest.DiagnosticId,
            CitizenId = serviceRequest.CitizenId,
            CitizenName = user?.FullName ?? "Unknown",
            ProblemTitle = serviceRequest.ProblemTitle,
            ProblemDescription = serviceRequest.ProblemDescription,
            ProblemCategory = serviceRequest.ProblemCategory,
            AdditionalImages = serviceRequest.AdditionalImages,
            PreferredProviderType = serviceRequest.PreferredProviderType,
            PreferredServiceDate = serviceRequest.PreferredServiceDate,
            PropertyType = serviceRequest.PropertyType,PropertyAddress = serviceRequest.PropertyAddress,ContactPhone = serviceRequest.ContactPhone,
            Status = serviceRequest.Status,
            QuotesCount = serviceRequest.QuotesCount,
            ViewsCount = serviceRequest.ViewsCount,
            ExpiresAt = serviceRequest.ExpiresAt,
            CreatedAt = serviceRequest.CreatedAt,
            UpdatedAt = serviceRequest.UpdatedAt
        };

        return Ok(response);
    }

    [HttpGet("my-requests")]
    //[Authorize(Roles = "CITIZEN")]
    public async Task<IActionResult> GetMyRequests()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var requests = await _serviceRequestRepository.GetByCitizenIdAsync(Guid.Parse(userId));
        var user = await _userRepository.GetByIdAsync(Guid.Parse(userId));

        var response = requests.Select(sr => new ServiceRequestResponseDto
        {
            RequestId = sr.RequestId,
            DiagnosticId = sr.DiagnosticId,
            CitizenId = sr.CitizenId,
            CitizenName = user?.FullName ?? "Unknown",
            ProblemTitle = sr.ProblemTitle,
            ProblemDescription = sr.ProblemDescription,
            ProblemCategory = sr.ProblemCategory,
            AdditionalImages = sr.AdditionalImages,
            PreferredProviderType = sr.PreferredProviderType,
            PreferredServiceDate = sr.PreferredServiceDate,
            PropertyType = sr.PropertyType,
            PropertyAddress = sr.PropertyAddress,
            ContactPhone = sr.ContactPhone,
            Status = sr.Status,
            QuotesCount = sr.QuotesCount,
            ViewsCount = sr.ViewsCount,
            ExpiresAt = sr.ExpiresAt,
            CreatedAt = sr.CreatedAt,
            UpdatedAt = sr.UpdatedAt
        });

        return Ok(response);
    }

    [HttpGet("active")]
    [AllowAnonymous]
    public async Task<IActionResult> GetActiveRequests()
    {
        var requests = await _serviceRequestRepository.GetActiveRequestsAsync();
        
        var response = new List<ServiceRequestResponseDto>();
        foreach (var sr in requests)
        {
            var user = await _userRepository.GetByIdAsync(sr.CitizenId);
            response.Add(new ServiceRequestResponseDto
            {
                RequestId = sr.RequestId,
                DiagnosticId = sr.DiagnosticId,
                CitizenId = sr.CitizenId,
                CitizenName = user?.FullName ?? "Unknown",
                ProblemTitle = sr.ProblemTitle,
                ProblemDescription = sr.ProblemDescription,
                ProblemCategory = sr.ProblemCategory,
                AdditionalImages = sr.AdditionalImages,
                PreferredProviderType = sr.PreferredProviderType,
                PreferredServiceDate = sr.PreferredServiceDate,
                PropertyType = sr.PropertyType,
                PropertyAddress = sr.PropertyAddress,
                ContactPhone = sr.ContactPhone,
                Status = sr.Status,
                QuotesCount = sr.QuotesCount,
                ViewsCount = sr.ViewsCount,
                ExpiresAt = sr.ExpiresAt,
                CreatedAt = sr.CreatedAt,
                UpdatedAt = sr.UpdatedAt
            });
        }

        return Ok(response);
    }

    [HttpGet("category/{category}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetRequestsByCategory(string category)
    {
        var requests = await _serviceRequestRepository.GetByCategoryAsync(category);
        
        var response = new List<ServiceRequestResponseDto>();
        foreach (var sr in requests)
        {
            var user = await _userRepository.GetByIdAsync(sr.CitizenId);
            response.Add(new ServiceRequestResponseDto
            {
                RequestId = sr.RequestId,
                DiagnosticId = sr.DiagnosticId,
                CitizenId = sr.CitizenId,
                CitizenName = user?.FullName ?? "Unknown",
                ProblemTitle = sr.ProblemTitle,
                ProblemDescription = sr.ProblemDescription,
                ProblemCategory = sr.ProblemCategory,
                AdditionalImages = sr.AdditionalImages,
                PreferredProviderType = sr.PreferredProviderType,
                PreferredServiceDate = sr.PreferredServiceDate,
                PropertyType = sr.PropertyType,
                PropertyAddress = sr.PropertyAddress,
                ContactPhone = sr.ContactPhone,
                Status = sr.Status,
                QuotesCount = sr.QuotesCount,
                ViewsCount = sr.ViewsCount,
                ExpiresAt = sr.ExpiresAt,
                CreatedAt = sr.CreatedAt,
                UpdatedAt = sr.UpdatedAt
            });
        }

        return Ok(response);
    }

    [HttpPut("{id}")]
    //[Authorize(Roles = "CITIZEN")]
    public async Task<IActionResult> UpdateServiceRequest(Guid id, [FromBody] UpdateServiceRequestDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var serviceRequest = await _serviceRequestRepository.GetByIdAsync(id);
        if (serviceRequest == null)
        {
            return NotFound(new { message = "Service request not found" });
        }

        if (serviceRequest.CitizenId != Guid.Parse(userId))
        {
            return Forbid();
        }

        if (serviceRequest.Status == RequestStatus.PROVIDER_SELECTED || serviceRequest.Status == RequestStatus.CANCELLED)
        {
            return BadRequest(new { message = "Cannot update request in current status" });
        }

        if (dto.ProblemTitle != null) serviceRequest.ProblemTitle = dto.ProblemTitle;
        if (dto.ProblemDescription != null) serviceRequest.ProblemDescription = dto.ProblemDescription;
        if (dto.ProblemCategory != null) serviceRequest.ProblemCategory = dto.ProblemCategory;
        if (dto.AdditionalImages != null) serviceRequest.AdditionalImages = dto.AdditionalImages;
        if (dto.PreferredProviderType != null) serviceRequest.PreferredProviderType = dto.PreferredProviderType;
        if (dto.PreferredServiceDate.HasValue) serviceRequest.PreferredServiceDate = dto.PreferredServiceDate;
        if (dto.PropertyType != null) serviceRequest.PropertyType = dto.PropertyType;
        if (dto.PropertyAddress != null) serviceRequest.PropertyAddress = dto.PropertyAddress;
        if (dto.ContactPhone != null) serviceRequest.ContactPhone = dto.ContactPhone;
        if (dto.ExpiresAt.HasValue) serviceRequest.ExpiresAt = dto.ExpiresAt;

        serviceRequest.UpdatedAt = DateTime.UtcNow;

        _serviceRequestRepository.Update(serviceRequest);
        await _serviceRequestRepository.SaveChangesAsync();

        var user = await _userRepository.GetByIdAsync(Guid.Parse(userId));
        var response = new ServiceRequestResponseDto
        {
            RequestId = serviceRequest.RequestId,
            DiagnosticId = serviceRequest.DiagnosticId,
            CitizenId = serviceRequest.CitizenId,
            CitizenName = user?.FullName ?? "Unknown",
            ProblemTitle = serviceRequest.ProblemTitle,
            ProblemDescription = serviceRequest.ProblemDescription,
            ProblemCategory = serviceRequest.ProblemCategory,
            AdditionalImages = serviceRequest.AdditionalImages,
            PreferredProviderType = serviceRequest.PreferredProviderType,
            PreferredServiceDate = serviceRequest.PreferredServiceDate,
            PropertyType = serviceRequest.PropertyType,PropertyAddress = serviceRequest.PropertyAddress,ContactPhone = serviceRequest.ContactPhone,
            Status = serviceRequest.Status,
            QuotesCount = serviceRequest.QuotesCount,
            ViewsCount = serviceRequest.ViewsCount,
            ExpiresAt = serviceRequest.ExpiresAt,
            CreatedAt = serviceRequest.CreatedAt,
            UpdatedAt = serviceRequest.UpdatedAt
        };

        return Ok(response);
    }

    [HttpDelete("{id}/cancel")]
    //[Authorize(Roles = "CITIZEN")]
    public async Task<IActionResult> CancelServiceRequest(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var serviceRequest = await _serviceRequestRepository.GetByIdAsync(id);
        if (serviceRequest == null)
        {
            return NotFound(new { message = "Service request not found" });
        }

        if (serviceRequest.CitizenId != Guid.Parse(userId))
        {
            return Forbid();
        }

        if (serviceRequest.Status == RequestStatus.CANCELLED)
        {
            return BadRequest(new { message = "Request is already cancelled" });
        }

        if (serviceRequest.Status == RequestStatus.PROVIDER_SELECTED)
        {
            return BadRequest(new { message = "Cannot cancel request with selected provider" });
        }

        serviceRequest.Status = RequestStatus.CANCELLED;
        serviceRequest.UpdatedAt = DateTime.UtcNow;

        _serviceRequestRepository.Update(serviceRequest);
        await _serviceRequestRepository.SaveChangesAsync();

        return Ok(new { message = "Service request cancelled successfully" });
    }

    [HttpPost("{id}/select-quote/{quoteId}")]
    //[Authorize(Roles = "CITIZEN")]
    public async Task<IActionResult> SelectQuote(Guid id, Guid quoteId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var serviceRequest = await _serviceRequestRepository.GetByIdAsync(id);
        if (serviceRequest == null)
        {
            return NotFound(new { message = "Service request not found" });
        }

        if (serviceRequest.CitizenId != Guid.Parse(userId))
        {
            return Forbid();
        }

        if (serviceRequest.Status != RequestStatus.OPEN && serviceRequest.Status != RequestStatus.QUOTES_RECEIVED)
        {
            return BadRequest(new { message = "Cannot select quote in current status" });
        }

        serviceRequest.Status = RequestStatus.PROVIDER_SELECTED;
        serviceRequest.UpdatedAt = DateTime.UtcNow;

        _serviceRequestRepository.Update(serviceRequest);
        await _serviceRequestRepository.SaveChangesAsync();

        return Ok(new { message = "Quote selected successfully" });
    }
}
