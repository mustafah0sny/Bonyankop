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
            services.AddScoped<IProviderProfileRepository, ProviderProfileRepository>();
            services.AddScoped<IPortfolioItemRepository, PortfolioItemRepository>();
            services.AddScoped<IDiagnosticRepository, DiagnosticRepository>();

            // Register custom services
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAiDiagnosticService, AiDiagnosticService>();

            return services;
        }
    }
}
