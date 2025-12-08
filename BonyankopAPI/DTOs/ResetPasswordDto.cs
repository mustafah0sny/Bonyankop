using System.ComponentModel.DataAnnotations;

namespace BonyankopAPI.DTOs
{
    public class ResetPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
