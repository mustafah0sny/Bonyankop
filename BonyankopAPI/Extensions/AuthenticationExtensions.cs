using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BonyankopAPI.Extensions
{
    public static class AuthenticationExtensions
    {
        public static IServiceCollection AddJwtAuthentication(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];

            var authBuilder = services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!))
                };
            });

            // Social login disabled - uncomment and configure when needed
            // Only add Google authentication if ClientId is configured
            // var googleClientId = configuration["Authentication:Google:ClientId"];
            // if (!string.IsNullOrWhiteSpace(googleClientId) && googleClientId != "YOUR_GOOGLE_CLIENT_ID.apps.googleusercontent.com")
            // {
            //     authBuilder.AddGoogle(options =>
            //     {
            //         options.ClientId = googleClientId;
            //         options.ClientSecret = configuration["Authentication:Google:ClientSecret"] ?? "";
            //     });
            // }

            // Only add Facebook authentication if AppId is configured
            // var facebookAppId = configuration["Authentication:Facebook:AppId"];
            // if (!string.IsNullOrWhiteSpace(facebookAppId) && facebookAppId != "YOUR_FACEBOOK_APP_ID")
            // {
            //     authBuilder.AddFacebook(options =>
            //     {
            //         options.AppId = facebookAppId;
            //         options.AppSecret = configuration["Authentication:Facebook:AppSecret"] ?? "";
            //     });
            // }

            services.AddAuthorization();

            return services;
        }
    }
}
