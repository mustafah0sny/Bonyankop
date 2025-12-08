using System.ComponentModel.DataAnnotations;

namespace BonyankopAPI.DTOs
{
    public class VerifyAccountDto
    {
        [Required]
        public string Token { get; set; } = string.Empty;
    }
}
