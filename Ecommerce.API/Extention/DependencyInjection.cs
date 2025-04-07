using Ecommerce.API.Services.Interfaces;
using Ecommerce.API.Services;
using Ecommerce.Infrastructure.Mapping;

namespace Ecommerce.API.Extention
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddExtentionServices(this IServiceCollection services)
        {
            //mapper
            services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);


            //service
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ICategoryService, CategoryService>();
            return services;
        }
    }
}
