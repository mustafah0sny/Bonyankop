using BonyankopAPI.Interfaces;
using BonyankopAPI.Services;
using BonyankopAPI.Repositories;

namespace BonyankopAPI.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Register repositories
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUserRepository, UserRepository>();

            // Register custom services
            services.AddScoped<ITokenService, TokenService>();

            return services;
        }
    }
}
