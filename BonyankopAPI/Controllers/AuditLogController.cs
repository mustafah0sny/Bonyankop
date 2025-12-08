using BonyankopAPI.DTOs;
using BonyankopAPI.Interfaces;
using BonyankopAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BonyankopAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "ADMIN,GOVERNMENT")]
public class AuditLogController : ControllerBase
{
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IUserRepository _userRepository;

    public AuditLogController(IAuditLogRepository auditLogRepository, IUserRepository userRepository)
    {
        _auditLogRepository = auditLogRepository;
        _userRepository = userRepository;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAuditLog(Guid id)
    {
        var log = await _auditLogRepository.GetByIdAsync(id);
        if (log == null)
        {
            return NotFound(new { message = "Audit log not found" });
        }

        var response = await MapToResponseDto(log);
        return Ok(response);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUser(Guid userId)
    {
        var logs = await _auditLogRepository.GetByUserIdAsync(userId);
        var response = new List<AuditLogResponseDto>();

        foreach (var log in logs)
        {
            response.Add(await MapToResponseDto(log));
        }

        return Ok(response);
    }

    [HttpGet("entity/{entityType}/{entityId}")]
    public async Task<IActionResult> GetByEntity(string entityType, Guid entityId)
    {
        var logs = await _auditLogRepository.GetByEntityAsync(entityType, entityId);
        var response = new List<AuditLogResponseDto>();

        foreach (var log in logs)
        {
            response.Add(await MapToResponseDto(log));
        }

        return Ok(response);
    }

    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] AuditLogSearchDto searchDto)
    {
        var logs = await _auditLogRepository.SearchAsync(
            searchDto.ActionType,
            searchDto.EntityType,
            searchDto.StartDate,
            searchDto.EndDate,
            searchDto.Limit
        );

        var response = new List<AuditLogResponseDto>();
        foreach (var log in logs)
        {
            response.Add(await MapToResponseDto(log));
        }

        return Ok(response);
    }

    [HttpGet("my-logs")]
    [Authorize]
    public async Task<IActionResult> GetMyLogs()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var logs = await _auditLogRepository.GetByUserIdAsync(Guid.Parse(userId));
        var response = new List<AuditLogResponseDto>();

        foreach (var log in logs)
        {
            response.Add(await MapToResponseDto(log));
        }

        return Ok(response);
    }

    private async Task<AuditLogResponseDto> MapToResponseDto(AuditLog log)
    {
        var user = log.UserId.HasValue ? await _userRepository.GetByIdAsync(log.UserId.Value) : null;

        return new AuditLogResponseDto
        {
            LogId = log.LogId,
            UserId = log.UserId,
            UserName = user?.FullName,
            ActionType = log.ActionType,
            EntityType = log.EntityType,
            EntityId = log.EntityId,
            ActionDescription = log.ActionDescription,
            OldValuesJson = log.OldValuesJson,
            NewValuesJson = log.NewValuesJson,
            IpAddress = log.IpAddress,
            UserAgent = log.UserAgent,
            RequestUrl = log.RequestUrl,
            ResponseStatus = log.ResponseStatus,
            ExecutionTimeMs = log.ExecutionTimeMs,
            ErrorMessage = log.ErrorMessage,
            SessionId = log.SessionId,
            CreatedAt = log.CreatedAt
        };
    }
}
