using System.ComponentModel.DataAnnotations;
using BonyankopAPI.Models;

namespace BonyankopAPI.DTOs;

public class SystemSettingsResponseDto
{
    public Guid SettingId { get; set; }
    public string SettingKey { get; set; } = string.Empty;
    public string? SettingValue { get; set; }
    public Models.DataType DataType { get; set; }
    public string? Category { get; set; }
    public string? Description { get; set; }
    public bool IsPublic { get; set; }
    public bool IsEditable { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateSystemSettingDto
{
    [Required]
    [MaxLength(100)]
    public string SettingKey { get; set; } = string.Empty;
    
    public string? SettingValue { get; set; }
    
    [Required]
    public Models.DataType DataType { get; set; }
    
    [MaxLength(50)]
    public string? Category { get; set; }
    
    [MaxLength(500)]
    public string? Description { get; set; }
    
    public bool IsPublic { get; set; } = false;
    public bool IsEditable { get; set; } = true;
}

public class UpdateSystemSettingDto
{
    public string? SettingValue { get; set; }
    public string? Category { get; set; }
    public string? Description { get; set; }
    public bool? IsPublic { get; set; }
}
