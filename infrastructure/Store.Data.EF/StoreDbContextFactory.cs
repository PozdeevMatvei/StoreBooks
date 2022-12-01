using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Store.DTO.EF
{
    public class StoreDbContextFactory
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public StoreDbContextFactory(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public StoreDbContext GetOrCreate(Type repositoryType)
        {
            var services = _httpContextAccessor.HttpContext.RequestServices;
            var dbContexts = services.GetService<Dictionary<Type, StoreDbContext>>();
            if (!dbContexts!.ContainsKey(repositoryType))
                dbContexts[repositoryType] = services.GetService<StoreDbContext>()!;

            return dbContexts[repositoryType];
        }
    }
}
