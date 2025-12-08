using BonyankopAPI.Models;

namespace BonyankopAPI.Interfaces;

public interface IAuditLogRepository : IRepository<AuditLog>
{
    Task<IEnumerable<AuditLog>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<AuditLog>> GetByEntityAsync(string entityType, Guid entityId);
    Task<IEnumerable<AuditLog>> SearchAsync(string? actionType, string? entityType, DateTime? startDate, DateTime? endDate, int limit = 100);
    Task LogActionAsync(Guid? userId, string actionType, string? entityType, Guid? entityId, string? actionDescription, string? oldValuesJson, string? newValuesJson, string? ipAddress, string? userAgent, string? requestUrl, int? responseStatus, int? executionTimeMs, string? errorMessage, string? sessionId);
}
