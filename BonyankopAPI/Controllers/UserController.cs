using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BonyankopAPI.DTOs;
using BonyankopAPI.Models;
using BonyankopAPI.Interfaces;
using BCrypt.Net;

namespace BonyankopAPI.Controllers
{
    /// <summary>
    /// Handles user authentication operations
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;

        public UserController(IUserRepository userRepository, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Register a new user account
        /// </summary>
        /// <param name="signUpDto">User registration details</param>
        /// <returns>JWT token and user information</returns>
        /// <response code="200">User successfully registered</response>
        /// <response code="400">Email already exists</response>
        /// <response code="500">Internal server error</response>
        [HttpPost("signup")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AuthResponseDto>> SignUp([FromBody] SignUpDto signUpDto)
        {
            try
            {
                // Check if user already exists
                if (await _userRepository.ExistsByEmailAsync(signUpDto.Email))
                {
                    return BadRequest(new { message = "Email already exists" });
                }

                // Hash the password
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(signUpDto.Password);

                // Create new user
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    UserName = signUpDto.Email,
                    Email = signUpDto.Email,
                    FullName = signUpDto.FullName,
                    PasswordHash = passwordHash,
                    Role = signUpDto.Role,
                    PhoneNumber = signUpDto.PhoneNumber,
                    IsVerified = false,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _userRepository.AddAsync(user);
                await _userRepository.SaveChangesAsync();

                // Generate JWT token
                var token = _tokenService.GenerateJwtToken(user);

                return Ok(new AuthResponseDto
                {
                    Token = token,
                    UserId = user.Id,
                    Email = user.Email!,
                    FullName = user.FullName,
                    Role = user.Role,
                    IsVerified = user.IsVerified
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during signup", error = ex.Message });
            }
        }

        /// <summary>
        /// Authenticate a user and return JWT token
        /// </summary>
        /// <param name="loginDto">User login credentials</param>
        /// <returns>JWT token and user information</returns>
        /// <response code="200">User successfully authenticated</response>
        /// <response code="401">Invalid email or password</response>
        /// <response code="500">Internal server error</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                // Find user by email
                var user = await _userRepository.GetByEmailAsync(loginDto.Email);

                if (user == null)
                {
                    return Unauthorized(new { message = "Invalid email or password" });
                }

                if (user.IsActive != true)
                {
                    return Unauthorized(new { message = "Account is deactivated" });
                }

                // Verify password
                if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                {
                    return Unauthorized(new { message = "Invalid email or password" });
                }

                // Update last login
                user.LastLoginAt = DateTime.UtcNow;
                user.UpdatedAt = DateTime.UtcNow;
                _userRepository.Update(user);
                await _userRepository.SaveChangesAsync();

                // Generate JWT token
                var token = _tokenService.GenerateJwtToken(user);

                return Ok(new AuthResponseDto
                {
                    Token = token,
                    UserId = user.Id,
                    Email = user.Email!,
                    FullName = user.FullName,
                    Role = user.Role,
                    IsVerified = user.IsVerified
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during login", error = ex.Message });
            }
        }

        /// <summary>
        /// Social login with Google, Facebook, or Apple
        /// </summary>
        /// <param name="socialLoginDto">Social login credentials</param>
        /// <returns>JWT token and user information</returns>
        /// <response code="200">Social login successful</response>
        /// <response code="400">Invalid token or provider</response>
        [HttpPost("social-login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthResponseDto>> SocialLogin([FromBody] SocialLoginDto socialLoginDto)
        {
            try
            {
                // Validate provider
                var provider = socialLoginDto.Provider.ToLower();
                if (provider != "google" && provider != "facebook" && provider != "apple")
                {
                    return BadRequest(new { message = "Invalid provider. Supported: google, facebook, apple" });
                }

                // In a real implementation, you would verify the IdToken with the provider's API
                // For now, we'll decode the token to extract user information
                // Mobile app should send the ID token obtained from the social provider SDK

                string email = "";
                string fullName = "";
                string providerId = "";

                // Parse the social provider token
                // Note: In production, use proper token validation libraries
                try
                {
                    var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                    var jsonToken = handler.ReadToken(socialLoginDto.IdToken) as System.IdentityModel.Tokens.Jwt.JwtSecurityToken;

                    if (jsonToken != null)
                    {
                        email = jsonToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value ?? "";
                        fullName = jsonToken.Claims.FirstOrDefault(c => c.Type == "name")?.Value ?? "";
                        providerId = jsonToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value ?? "";

                        // Google specific claims
                        if (string.IsNullOrEmpty(fullName))
                        {
                            var givenName = jsonToken.Claims.FirstOrDefault(c => c.Type == "given_name")?.Value ?? "";
                            var familyName = jsonToken.Claims.FirstOrDefault(c => c.Type == "family_name")?.Value ?? "";
                            fullName = $"{givenName} {familyName}".Trim();
                        }
                    }
                }
                catch
                {
                    return BadRequest(new { message = "Invalid ID token" });
                }

                if (string.IsNullOrEmpty(email))
                {
                    return BadRequest(new { message = "Email not found in token" });
                }

                // Check if user exists
                var user = await _userRepository.GetByEmailAsync(email);

                if (user == null)
                {
                    // Create new user from social login
                    UserRole userRole = UserRole.CITIZEN; // Default role

                    // If role is provided and valid, use it
                    if (!string.IsNullOrEmpty(socialLoginDto.Role) && 
                        Enum.TryParse<UserRole>(socialLoginDto.Role, true, out var parsedRole))
                    {
                        userRole = parsedRole;
                    }

                    user = new User
                    {
                        Id = Guid.NewGuid(),
                        UserName = email,
                        Email = email,
                        FullName = string.IsNullOrEmpty(fullName) ? email.Split('@')[0] : fullName,
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString()), // Random password
                        Role = userRole,
                        IsVerified = true, // Social login users are pre-verified
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        LastLoginAt = DateTime.UtcNow
                    };

                    await _userRepository.AddAsync(user);
                    await _userRepository.SaveChangesAsync();
                }
                else
                {
                    // Update last login
                    if (user.IsActive != true)
                    {
                        return BadRequest(new { message = "Account is deactivated" });
                    }

                    user.LastLoginAt = DateTime.UtcNow;
                    user.UpdatedAt = DateTime.UtcNow;
                    _userRepository.Update(user);
                    await _userRepository.SaveChangesAsync();
                }

                // Generate JWT token
                var token = _tokenService.GenerateJwtToken(user);

                return Ok(new AuthResponseDto
                {
                    Token = token,
                    UserId = user.Id,
                    Email = user.Email!,
                    FullName = user.FullName,
                    Role = user.Role,
                    IsVerified = user.IsVerified
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during social login", error = ex.Message });
            }
        }

        /// <summary>
        /// Logout user (client-side token removal)
        /// </summary>
        /// <returns>Success message</returns>
        /// <response code="200">User successfully logged out</response>
        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult Logout()
        {
            return Ok(new { message = "Logged out successfully" });
        }

        /// <summary>
        /// Verify user account with token
        /// </summary>
        /// <param name="verifyDto">Verification token</param>
        /// <returns>Success message</returns>
        /// <response code="200">Account verified successfully</response>
        /// <response code="400">Invalid or expired token</response>
        [HttpPost("verify")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> VerifyAccount([FromBody] VerifyAccountDto verifyDto)
        {
            try
            {
                var userId = Guid.Parse(verifyDto.Token);
                var user = await _userRepository.GetByIdAsync(userId);

                if (user == null)
                {
                    return BadRequest(new { message = "Invalid verification token" });
                }

                user.IsVerified = true;
                user.UpdatedAt = DateTime.UtcNow;
                _userRepository.Update(user);
                await _userRepository.SaveChangesAsync();

                return Ok(new { message = "Account verified successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Invalid or expired token", error = ex.Message });
            }
        }

        /// <summary>
        /// Request password reset
        /// </summary>
        /// <param name="resetDto">User email</param>
        /// <returns>Success message</returns>
        /// <response code="200">Reset link sent (if email exists)</response>
        [HttpPost("reset-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordDto resetDto)
        {
            try
            {
                var user = await _userRepository.GetByEmailAsync(resetDto.Email);

                if (user != null)
                {
                    // TODO: Generate reset token and send email
                }

                return Ok(new { message = "If the email exists, a reset link has been sent" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }
    }
}
