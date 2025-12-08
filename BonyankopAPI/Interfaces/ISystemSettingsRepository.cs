using BonyankopAPI.Models;

namespace BonyankopAPI.Interfaces;

public interface ISystemSettingsRepository : IRepository<SystemSettings>
{
    Task<SystemSettings?> GetByKeyAsync(string key);
    Task<IEnumerable<SystemSettings>> GetByCategoryAsync(string category);
    Task<IEnumerable<SystemSettings>> GetPublicSettingsAsync();
    Task<bool> SetValueAsync(string key, string value);
    Task<T?> GetValueAsync<T>(string key);
}
