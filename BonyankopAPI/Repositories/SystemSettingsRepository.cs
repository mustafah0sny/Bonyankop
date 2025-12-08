using BonyankopAPI.Data;
using BonyankopAPI.Interfaces;
using BonyankopAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace BonyankopAPI.Repositories;

public class SystemSettingsRepository : Repository<SystemSettings>, ISystemSettingsRepository
{
    public SystemSettingsRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<SystemSettings?> GetByKeyAsync(string key)
    {
        return await _context.Set<SystemSettings>()
            .FirstOrDefaultAsync(s => s.SettingKey == key);
    }

    public async Task<IEnumerable<SystemSettings>> GetByCategoryAsync(string category)
    {
        return await _context.Set<SystemSettings>()
            .Where(s => s.Category == category)
            .OrderBy(s => s.SettingKey)
            .ToListAsync();
    }

    public async Task<IEnumerable<SystemSettings>> GetPublicSettingsAsync()
    {
        return await _context.Set<SystemSettings>()
            .Where(s => s.IsPublic)
            .OrderBy(s => s.Category)
            .ThenBy(s => s.SettingKey)
            .ToListAsync();
    }

    public async Task<bool> SetValueAsync(string key, string value)
    {
        var setting = await GetByKeyAsync(key);
        if (setting == null || !setting.IsEditable)
        {
            return false;
        }

        setting.SettingValue = value;
        setting.UpdatedAt = DateTime.UtcNow;
        
        Update(setting);
        await SaveChangesAsync();
        return true;
    }

    public async Task<T?> GetValueAsync<T>(string key)
    {
        var setting = await GetByKeyAsync(key);
        if (setting == null || string.IsNullOrEmpty(setting.SettingValue))
        {
            return default;
        }

        try
        {
            return setting.DataType switch
            {
                DataType.STRING => (T)(object)setting.SettingValue,
                DataType.NUMBER => (T)Convert.ChangeType(setting.SettingValue, typeof(T)),
                DataType.BOOLEAN => (T)(object)bool.Parse(setting.SettingValue),
                DataType.JSON => JsonSerializer.Deserialize<T>(setting.SettingValue),
                _ => default
            };
        }
        catch
        {
            return default;
        }
    }
}
