using System.ComponentModel.DataAnnotations;

namespace BonyankopAPI.Models;

public class SystemSettings
{
    public Guid SettingId { get; set; } = Guid.NewGuid();
    
    [Required]
    [MaxLength(100)]
    public string SettingKey { get; set; } = string.Empty;
    
    public string? SettingValue { get; set; }
    public DataType DataType { get; set; } = DataType.STRING;
    
    [MaxLength(50)]
    public string? Category { get; set; }
    
    [MaxLength(500)]
    public string? Description { get; set; }
    
    public bool IsPublic { get; set; } = false;
    public bool IsEditable { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
