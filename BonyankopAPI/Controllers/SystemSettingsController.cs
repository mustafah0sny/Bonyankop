using BonyankopAPI.DTOs;
using BonyankopAPI.Interfaces;
using BonyankopAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BonyankopAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SystemSettingsController : ControllerBase
{
    private readonly ISystemSettingsRepository _settingsRepository;

    public SystemSettingsController(ISystemSettingsRepository settingsRepository)
    {
        _settingsRepository = settingsRepository;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetPublicSettings()
    {
        var settings = await _settingsRepository.GetPublicSettingsAsync();
        var response = settings.Select(MapToResponseDto);
        return Ok(response);
    }

    [HttpGet("all")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> GetAllSettings()
    {
        var settings = await _settingsRepository.GetAllAsync();
        var response = settings.Select(MapToResponseDto);
        return Ok(response);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> GetSetting(Guid id)
    {
        var setting = await _settingsRepository.GetByIdAsync(id);
        if (setting == null)
        {
            return NotFound(new { message = "Setting not found" });
        }

        return Ok(MapToResponseDto(setting));
    }

    [HttpGet("key/{key}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> GetByKey(string key)
    {
        var setting = await _settingsRepository.GetByKeyAsync(key);
        if (setting == null)
        {
            return NotFound(new { message = "Setting not found" });
        }

        return Ok(MapToResponseDto(setting));
    }

    [HttpGet("category/{category}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> GetByCategory(string category)
    {
        var settings = await _settingsRepository.GetByCategoryAsync(category);
        var response = settings.Select(MapToResponseDto);
        return Ok(response);
    }

    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> CreateSetting([FromBody] CreateSystemSettingDto dto)
    {
        var existing = await _settingsRepository.GetByKeyAsync(dto.SettingKey);
        if (existing != null)
        {
            return BadRequest(new { message = "Setting with this key already exists" });
        }

        var setting = new SystemSettings
        {
            SettingKey = dto.SettingKey,
            SettingValue = dto.SettingValue,
            DataType = dto.DataType,
            Category = dto.Category,
            Description = dto.Description,
            IsPublic = dto.IsPublic,
            IsEditable = dto.IsEditable
        };

        await _settingsRepository.AddAsync(setting);
        await _settingsRepository.SaveChangesAsync();

        return CreatedAtAction(nameof(GetSetting), new { id = setting.SettingId }, MapToResponseDto(setting));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> UpdateSetting(Guid id, [FromBody] UpdateSystemSettingDto dto)
    {
        var setting = await _settingsRepository.GetByIdAsync(id);
        if (setting == null)
        {
            return NotFound(new { message = "Setting not found" });
        }

        if (!setting.IsEditable)
        {
            return BadRequest(new { message = "This setting is not editable" });
        }

        if (dto.SettingValue != null) setting.SettingValue = dto.SettingValue;
        if (dto.Category != null) setting.Category = dto.Category;
        if (dto.Description != null) setting.Description = dto.Description;
        if (dto.IsPublic.HasValue) setting.IsPublic = dto.IsPublic.Value;

        setting.UpdatedAt = DateTime.UtcNow;

        _settingsRepository.Update(setting);
        await _settingsRepository.SaveChangesAsync();

        return Ok(MapToResponseDto(setting));
    }

    [HttpPut("key/{key}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> UpdateSettingValue(string key, [FromBody] string value)
    {
        var success = await _settingsRepository.SetValueAsync(key, value);
        if (!success)
        {
            return BadRequest(new { message = "Setting not found or not editable" });
        }

        return Ok(new { message = "Setting updated successfully" });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> DeleteSetting(Guid id)
    {
        var setting = await _settingsRepository.GetByIdAsync(id);
        if (setting == null)
        {
            return NotFound(new { message = "Setting not found" });
        }

        if (!setting.IsEditable)
        {
            return BadRequest(new { message = "This setting cannot be deleted" });
        }

        _settingsRepository.Remove(setting);
        await _settingsRepository.SaveChangesAsync();

        return Ok(new { message = "Setting deleted successfully" });
    }

    private SystemSettingsResponseDto MapToResponseDto(SystemSettings setting)
    {
        return new SystemSettingsResponseDto
        {
            SettingId = setting.SettingId,
            SettingKey = setting.SettingKey,
            SettingValue = setting.SettingValue,
            DataType = setting.DataType,
            Category = setting.Category,
            Description = setting.Description,
            IsPublic = setting.IsPublic,
            IsEditable = setting.IsEditable,
            CreatedAt = setting.CreatedAt,
            UpdatedAt = setting.UpdatedAt
        };
    }
}
