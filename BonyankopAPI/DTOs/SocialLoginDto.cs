using System.ComponentModel.DataAnnotations;

namespace BonyankopAPI.DTOs
{
    /// <summary>
    /// DTO for social login (Google, Facebook, Apple)
    /// </summary>
    public class SocialLoginDto
    {
        /// <summary>
        /// Social provider (google, facebook, apple)
        /// </summary>
        [Required(ErrorMessage = "Provider is required")]
        public string Provider { get; set; } = string.Empty;

        /// <summary>
        /// ID Token from the social provider
        /// </summary>
        [Required(ErrorMessage = "ID Token is required")]
        public string IdToken { get; set; } = string.Empty;

        /// <summary>
        /// Optional: User's role selection during social signup
        /// </summary>
        public string? Role { get; set; }
    }
}
