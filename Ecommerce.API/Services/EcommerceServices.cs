

namespace Ecommerce.API.Services
{
    public class EcommerceServices(EcommerceDbContext dbContext, ILogger<EcommerceServices> logger)
    {
        public EcommerceDbContext DbContext { get; } = dbContext;
        public ILogger<EcommerceServices> Logger => logger;
    }
}
