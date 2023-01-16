using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Store.Web.App;

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

        public static IServiceCollection AddIdentityOptions(this IServiceCollection services)
        {
            services.AddIdentity<User, UserRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 4;
            }).AddEntityFrameworkStores<StoreDbContext>();
            return services;
        }
    }
}
