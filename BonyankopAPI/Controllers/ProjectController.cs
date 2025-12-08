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
public class ProjectController : ControllerBase
{
    private readonly IProjectRepository _projectRepository;
    private readonly IQuoteRepository _quoteRepository;
    private readonly IServiceRequestRepository _serviceRequestRepository;
    private readonly IProviderProfileRepository _providerProfileRepository;
    private readonly IUserRepository _userRepository;

    public ProjectController(
        IProjectRepository projectRepository,
        IQuoteRepository quoteRepository,
        IServiceRequestRepository serviceRequestRepository,
        IProviderProfileRepository providerProfileRepository,
        IUserRepository userRepository)
    {
        _projectRepository = projectRepository;
        _quoteRepository = quoteRepository;
        _serviceRequestRepository = serviceRequestRepository;
        _providerProfileRepository = providerProfileRepository;
        _userRepository = userRepository;
    }

    [HttpPost]
    [Authorize(Roles = "CITIZEN")]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        // Get and validate quote
        var quote = await _quoteRepository.GetByIdAsync(dto.QuoteId);
        if (quote == null)
        {
            return NotFound(new { message = "Quote not found" });
        }

        if (quote.Status != QuoteStatus.PENDING)
        {
            return BadRequest(new { message = "Only pending quotes can be accepted" });
        }

        // Get service request
        var serviceRequest = await _serviceRequestRepository.GetByIdAsync(quote.RequestId);
        if (serviceRequest == null || serviceRequest.CitizenId != Guid.Parse(userId))
        {
            return Forbid();
        }

        // Check if project already exists for this quote
        var existingProject = await _projectRepository.GetByQuoteIdAsync(dto.QuoteId);
        if (existingProject != null)
        {
            return BadRequest(new { message = "Project already exists for this quote" });
        }

        // Create project
        var project = new Project
        {
            RequestId = quote.RequestId,
            QuoteId = quote.QuoteId,
            CitizenId = Guid.Parse(userId),
            ProviderId = quote.ProviderId,
            ProjectTitle = dto.ProjectTitle ?? serviceRequest.ProblemTitle,
            ProjectDescription = dto.ProjectDescription ?? serviceRequest.ProblemDescription,
            ScheduledStartDate = dto.ScheduledStartDate,
            ScheduledEndDate = dto.ScheduledEndDate,
            AgreedCost = quote.EstimatedCost
        };

        await _projectRepository.AddAsync(project);
        await _projectRepository.SaveChangesAsync();

        // Update quote status
        quote.Status = QuoteStatus.ACCEPTED;
        quote.AcceptedAt = DateTime.UtcNow;
        _quoteRepository.Update(quote);
        await _quoteRepository.SaveChangesAsync();

        // Update service request status
        serviceRequest.Status = RequestStatus.PROVIDER_SELECTED;
        serviceRequest.UpdatedAt = DateTime.UtcNow;
        _serviceRequestRepository.Update(serviceRequest);
        await _serviceRequestRepository.SaveChangesAsync();

        var response = await MapToResponseDto(project);
        return CreatedAtAction(nameof(GetProject), new { id = project.ProjectId }, response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProject(Guid id)
    {
        var project = await _projectRepository.GetByIdAsync(id);
        if (project == null)
        {
            return NotFound(new { message = "Project not found" });
        }

        var response = await MapToResponseDto(project);
        return Ok(response);
    }

    [HttpGet("my-projects")]
    [Authorize(Roles = "CITIZEN")]
    public async Task<IActionResult> GetMyProjects()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var projects = await _projectRepository.GetByCitizenIdAsync(Guid.Parse(userId));
        var response = new List<ProjectResponseDto>();

        foreach (var project in projects)
        {
            response.Add(await MapToResponseDto(project));
        }

        return Ok(response);
    }

    [HttpGet("provider-projects")]
    [Authorize(Roles = "COMPANY,ENGINEER")]
    public async Task<IActionResult> GetProviderProjects()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var providerProfile = await _providerProfileRepository.GetByUserIdAsync(Guid.Parse(userId));
        if (providerProfile == null)
        {
            return BadRequest(new { message = "Provider profile not found" });
        }

        var projects = await _projectRepository.GetByProviderIdAsync(providerProfile.ProviderId);
        var response = new List<ProjectResponseDto>();

        foreach (var project in projects)
        {
            response.Add(await MapToResponseDto(project));
        }

        return Ok(response);
    }

    [HttpGet("active")]
    [Authorize(Roles = "ADMIN,GOVERNMENT")]
    public async Task<IActionResult> GetActiveProjects()
    {
        var projects = await _projectRepository.GetActiveProjectsAsync();
        var response = new List<ProjectResponseDto>();

        foreach (var project in projects)
        {
            response.Add(await MapToResponseDto(project));
        }

        return Ok(response);
    }

    [HttpGet("overdue")]
    [Authorize(Roles = "ADMIN,GOVERNMENT")]
    public async Task<IActionResult> GetOverdueProjects()
    {
        var projects = await _projectRepository.GetOverdueProjectsAsync();
        var response = new List<ProjectResponseDto>();

        foreach (var project in projects)
        {
            response.Add(await MapToResponseDto(project));
        }

        return Ok(response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProject(Guid id, [FromBody] UpdateProjectDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var project = await _projectRepository.GetByIdAsync(id);
        if (project == null)
        {
            return NotFound(new { message = "Project not found" });
        }

        // Check permissions
        var userRoles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        var isProvider = false;

        if (userRoles.Contains("COMPANY") || userRoles.Contains("ENGINEER"))
        {
            var providerProfile = await _providerProfileRepository.GetByUserIdAsync(Guid.Parse(userId));
            isProvider = providerProfile != null && providerProfile.ProviderId == project.ProviderId;
        }

        var isCitizen = project.CitizenId == Guid.Parse(userId);

        if (!isCitizen && !isProvider && !userRoles.Contains("ADMIN"))
        {
            return Forbid();
        }

        // Update fields
        if (dto.Status.HasValue) project.Status = dto.Status.Value;
        if (dto.ScheduledStartDate.HasValue) project.ScheduledStartDate = dto.ScheduledStartDate;
        if (dto.ActualStartDate.HasValue) project.ActualStartDate = dto.ActualStartDate;
        if (dto.ScheduledEndDate.HasValue) project.ScheduledEndDate = dto.ScheduledEndDate;
        if (dto.ActualCompletionDate.HasValue) project.ActualCompletionDate = dto.ActualCompletionDate;
        if (dto.ActualCost.HasValue) project.ActualCost = dto.ActualCost;
        if (dto.CostDifferenceReason != null) project.CostDifferenceReason = dto.CostDifferenceReason;
        if (dto.PaymentStatus.HasValue) project.PaymentStatus = dto.PaymentStatus.Value;
        if (dto.TechnicalReportUrl != null) project.TechnicalReportUrl = dto.TechnicalReportUrl;
        if (dto.CompletionCertificateUrl != null) project.CompletionCertificateUrl = dto.CompletionCertificateUrl;
        if (dto.WarrantyStartDate.HasValue) project.WarrantyStartDate = dto.WarrantyStartDate;
        if (dto.WarrantyEndDate.HasValue) project.WarrantyEndDate = dto.WarrantyEndDate;
        if (dto.CitizenSatisfaction != null) project.CitizenSatisfaction = dto.CitizenSatisfaction;

        project.UpdatedAt = DateTime.UtcNow;

        _projectRepository.Update(project);
        await _projectRepository.SaveChangesAsync();

        var response = await MapToResponseDto(project);
        return Ok(response);
    }

    [HttpPost("{id}/work-notes")]
    public async Task<IActionResult> AddWorkNote(Guid id, [FromBody] AddWorkNoteDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var project = await _projectRepository.GetByIdAsync(id);
        if (project == null)
        {
            return NotFound(new { message = "Project not found" });
        }

        // Check permissions
        var userRoles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        var isProvider = false;

        if (userRoles.Contains("COMPANY") || userRoles.Contains("ENGINEER"))
        {
            var providerProfile = await _providerProfileRepository.GetByUserIdAsync(Guid.Parse(userId));
            isProvider = providerProfile != null && providerProfile.ProviderId == project.ProviderId;
        }

        var isCitizen = project.CitizenId == Guid.Parse(userId);

        if (!isCitizen && !isProvider)
        {
            return Forbid();
        }

        var user = await _userRepository.GetByIdAsync(Guid.Parse(userId));
        var workNotes = project.WorkNotes;
        workNotes.Add(new WorkNote
        {
            Timestamp = DateTime.UtcNow,
            Author = user?.FullName ?? "Unknown",
            Note = dto.Note,
            Images = dto.Images
        });

        project.WorkNotes = workNotes;
        project.UpdatedAt = DateTime.UtcNow;

        _projectRepository.Update(project);
        await _projectRepository.SaveChangesAsync();

        return Ok(new { message = "Work note added successfully" });
    }

    [HttpPost("{id}/images/{phase}")]
    public async Task<IActionResult> UploadImages(Guid id, string phase, [FromBody] List<string> images)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var project = await _projectRepository.GetByIdAsync(id);
        if (project == null)
        {
            return NotFound(new { message = "Project not found" });
        }

        // Check permissions (provider only)
        var userRoles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        if (!userRoles.Contains("COMPANY") && !userRoles.Contains("ENGINEER") && !userRoles.Contains("ADMIN"))
        {
            return Forbid();
        }

        var providerProfile = await _providerProfileRepository.GetByUserIdAsync(Guid.Parse(userId));
        if (providerProfile == null || (providerProfile.ProviderId != project.ProviderId && !userRoles.Contains("ADMIN")))
        {
            return Forbid();
        }

        switch (phase.ToLower())
        {
            case "before":
                project.BeforeImages.AddRange(images);
                break;
            case "during":
                project.DuringImages.AddRange(images);
                break;
            case "after":
                project.AfterImages.AddRange(images);
                break;
            default:
                return BadRequest(new { message = "Invalid phase. Must be 'before', 'during', or 'after'" });
        }

        project.UpdatedAt = DateTime.UtcNow;

        _projectRepository.Update(project);
        await _projectRepository.SaveChangesAsync();

        return Ok(new { message = $"{phase.ToUpper()} images uploaded successfully" });
    }

    [HttpPost("{id}/start")]
    [Authorize(Roles = "COMPANY,ENGINEER")]
    public async Task<IActionResult> StartProject(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var project = await _projectRepository.GetByIdAsync(id);
        if (project == null)
        {
            return NotFound(new { message = "Project not found" });
        }

        var providerProfile = await _providerProfileRepository.GetByUserIdAsync(Guid.Parse(userId));
        if (providerProfile == null || providerProfile.ProviderId != project.ProviderId)
        {
            return Forbid();
        }

        if (project.Status != ProjectStatus.SCHEDULED)
        {
            return BadRequest(new { message = "Only scheduled projects can be started" });
        }

        project.Status = ProjectStatus.IN_PROGRESS;
        project.ActualStartDate = DateTime.UtcNow;
        project.UpdatedAt = DateTime.UtcNow;

        _projectRepository.Update(project);
        await _projectRepository.SaveChangesAsync();

        return Ok(new { message = "Project started successfully" });
    }

    [HttpPost("{id}/complete")]
    [Authorize(Roles = "COMPANY,ENGINEER")]
    public async Task<IActionResult> CompleteProject(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var project = await _projectRepository.GetByIdAsync(id);
        if (project == null)
        {
            return NotFound(new { message = "Project not found" });
        }

        var providerProfile = await _providerProfileRepository.GetByUserIdAsync(Guid.Parse(userId));
        if (providerProfile == null || providerProfile.ProviderId != project.ProviderId)
        {
            return Forbid();
        }

        if (project.Status != ProjectStatus.IN_PROGRESS)
        {
            return BadRequest(new { message = "Only in-progress projects can be completed" });
        }

        project.Status = ProjectStatus.COMPLETED;
        project.ActualCompletionDate = DateTime.UtcNow;
        project.UpdatedAt = DateTime.UtcNow;

        _projectRepository.Update(project);
        await _projectRepository.SaveChangesAsync();

        return Ok(new { message = "Project completed successfully" });
    }

    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> CancelProject(Guid id, [FromBody] string reason)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var project = await _projectRepository.GetByIdAsync(id);
        if (project == null)
        {
            return NotFound(new { message = "Project not found" });
        }

        // Check permissions
        var userRoles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        var isProvider = false;

        if (userRoles.Contains("COMPANY") || userRoles.Contains("ENGINEER"))
        {
            var providerProfile = await _providerProfileRepository.GetByUserIdAsync(Guid.Parse(userId));
            isProvider = providerProfile != null && providerProfile.ProviderId == project.ProviderId;
        }

        var isCitizen = project.CitizenId == Guid.Parse(userId);

        if (!isCitizen && !isProvider && !userRoles.Contains("ADMIN"))
        {
            return Forbid();
        }

        if (project.Status == ProjectStatus.COMPLETED)
        {
            return BadRequest(new { message = "Completed projects cannot be cancelled" });
        }

        project.Status = ProjectStatus.CANCELLED;
        project.UpdatedAt = DateTime.UtcNow;

        _projectRepository.Update(project);
        await _projectRepository.SaveChangesAsync();

        return Ok(new { message = "Project cancelled successfully", reason });
    }

    private async Task<ProjectResponseDto> MapToResponseDto(Project project)
    {
        var citizen = await _userRepository.GetByIdAsync(project.CitizenId);
        var provider = await _providerProfileRepository.GetByIdAsync(project.ProviderId);
        var providerUser = provider != null ? await _userRepository.GetByIdAsync(provider.UserId) : null;

        return new ProjectResponseDto
        {
            ProjectId = project.ProjectId,
            RequestId = project.RequestId,
            QuoteId = project.QuoteId,
            CitizenId = project.CitizenId,
            CitizenName = citizen?.FullName ?? "Unknown",
            ProviderId = project.ProviderId,
            ProviderName = providerUser?.FullName ?? provider?.BusinessName ?? "Unknown",
            ProjectTitle = project.ProjectTitle,
            ProjectDescription = project.ProjectDescription,
            Status = project.Status,
            ScheduledStartDate = project.ScheduledStartDate,
            ActualStartDate = project.ActualStartDate,
            ScheduledEndDate = project.ScheduledEndDate,
            ActualCompletionDate = project.ActualCompletionDate,
            AgreedCost = project.AgreedCost,
            ActualCost = project.ActualCost,
            CostDifferenceReason = project.CostDifferenceReason,
            PaymentStatus = project.PaymentStatus,
            WorkNotes = project.WorkNotes.Select(wn => new WorkNoteDto
            {
                Timestamp = wn.Timestamp,
                Author = wn.Author,
                Note = wn.Note,
                Images = wn.Images
            }).ToList(),
            BeforeImages = project.BeforeImages,
            DuringImages = project.DuringImages,
            AfterImages = project.AfterImages,
            TechnicalReportUrl = project.TechnicalReportUrl,
            CompletionCertificateUrl = project.CompletionCertificateUrl,
            WarrantyStartDate = project.WarrantyStartDate,
            WarrantyEndDate = project.WarrantyEndDate,
            CitizenSatisfaction = project.CitizenSatisfaction,
            Duration = project.CalculateDuration(),
            IsOverdue = project.IsOverdue(),
            CreatedAt = project.CreatedAt,
            UpdatedAt = project.UpdatedAt
        };
    }
}
