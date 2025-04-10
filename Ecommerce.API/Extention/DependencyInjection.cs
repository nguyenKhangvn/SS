using Ecommerce.API.Services.Interfaces;
using Ecommerce.API.Services;
using Ecommerce.Infrastructure.Mapping;
using Microsoft.AspNetCore.Identity;

namespace Ecommerce.API.Extention
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddExtentionServices(this IServiceCollection services)
        {
            //mapper
            services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);


            //service
            // category
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ICategoryService, CategoryService>();
            // store location
            services.AddScoped<IStoreLocationRepository, StoreLocationRepository>();
            services.AddScoped<IStoreLocationService, StoreLocationService>();
            // Post
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<IPostService, PostService>();
            // User
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserService, UserService>();
            // Manufacturer
            services.AddScoped<IManufacturerRepository, ManufacturerRepository>();
            services.AddScoped<IManufacturerService, ManufacturerService>();
            // Address
            services.AddScoped<IAddressService, AddressService>();
            services.AddScoped<IAddressRepository, AddressRepository>();
            //order
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderService, OrderService>();
            //product
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductService, ProductService>();

            // coupon
            services.AddScoped<ICouponRepository, CouponRepository>();
            services.AddScoped<ICouponService, CouponService>();

            // order item
            services.AddScoped<IOrderItemRepository, OrderItemRepository>();
            services.AddScoped<IOrderItemService, OrderItemService>();
            return services;
        }
    }
}
