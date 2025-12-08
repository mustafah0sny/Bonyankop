using System.ComponentModel.DataAnnotations;

namespace BonyankopAPI.DTOs;

public class AddWorkNoteDto
{
    [Required(ErrorMessage = "Note is required")]
    [StringLength(2000, ErrorMessage = "Note cannot exceed 2000 characters")]
    public string Note { get; set; } = string.Empty;

    public List<string> Images { get; set; } = new();
}
