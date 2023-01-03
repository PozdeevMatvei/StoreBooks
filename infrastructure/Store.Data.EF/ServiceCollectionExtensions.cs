using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Store.DTO.EF
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEF(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<StoreDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            },
            ServiceLifetime.Transient
            );

            services.AddScoped<Dictionary<Type, StoreDbContext>>();
            services.AddSingleton<StoreDbContextFactory>();

            return services;
        }                
    }
}
