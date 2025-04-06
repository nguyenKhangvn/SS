
using Ecommerce.API.Dtos;
using Ecommerce.API.Interfaces;

namespace Ecommerce.API.Apis
{
    public static class ProductApi
    {
        public static IEndpointRouteBuilder MapProductApi(this IEndpointRouteBuilder builder)
        {
            var vApi = builder.NewVersionedApi("ecommerce");
            var v1 = vApi.MapGroup("api/v{version:apiVersion}/ecommerce").HasApiVersion(1, 0);

            //v1.MapPost("/products", (IProductRepository productService, Product createProductDto) => productService.AddProductAsync(createProductDto))
            //    .WithName("CreateProduct")
            //    .Produces<Product>(StatusCodes.Status201Created)
            //    .Produces(StatusCodes.Status400BadRequest)
            //    .WithTags("Products");

            v1.MapGet("/products", (IProductRepository productService) => productService.GetProductsAsync());
            return builder;
        }
    }
}
