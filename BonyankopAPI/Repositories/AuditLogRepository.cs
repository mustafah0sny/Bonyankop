using BonyankopAPI.Data;
using BonyankopAPI.Interfaces;
using BonyankopAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BonyankopAPI.Repositories;

public class AuditLogRepository : Repository<AuditLog>, IAuditLogRepository
{
    public AuditLogRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<AuditLog>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Set<AuditLog>()
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<AuditLog>> GetByEntityAsync(string entityType, Guid entityId)
    {
        return await _context.Set<AuditLog>()
            .Where(a => a.EntityType == entityType && a.EntityId == entityId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<AuditLog>> SearchAsync(string? actionType, string? entityType, DateTime? startDate, DateTime? endDate, int limit = 100)
    {
        var query = _context.Set<AuditLog>().AsQueryable();

        if (!string.IsNullOrEmpty(actionType))
        {
            query = query.Where(a => a.ActionType == actionType);
        }

        if (!string.IsNullOrEmpty(entityType))
        {
            query = query.Where(a => a.EntityType == entityType);
        }

        if (startDate.HasValue)
        {
            query = query.Where(a => a.CreatedAt >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(a => a.CreatedAt <= endDate.Value);
        }

        return await query
            .OrderByDescending(a => a.CreatedAt)
            .Take(limit)
            .ToListAsync();
    }

    public async Task LogActionAsync(
        Guid? userId, 
        string actionType, 
        string? entityType, 
        Guid? entityId, 
        string? actionDescription, 
        string? oldValuesJson, 
        string? newValuesJson, 
        string? ipAddress, 
        string? userAgent, 
        string? requestUrl, 
        int? responseStatus, 
        int? executionTimeMs, 
        string? errorMessage, 
        string? sessionId)
    {
        var log = new AuditLog
        {
            UserId = userId,
            ActionType = actionType,
            EntityType = entityType,
            EntityId = entityId,
            ActionDescription = actionDescription,
            OldValuesJson = oldValuesJson,
            NewValuesJson = newValuesJson,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            RequestUrl = requestUrl,
            ResponseStatus = responseStatus,
            ExecutionTimeMs = executionTimeMs,
            ErrorMessage = errorMessage,
            SessionId = sessionId
        };

        await AddAsync(log);
        await SaveChangesAsync();
    }
}
