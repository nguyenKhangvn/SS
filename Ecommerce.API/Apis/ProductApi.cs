
using Ecommerce.Infrastructure.Dtos;
using Ecommerce.Infrastructure.Models.Dtos;

namespace Ecommerce.API.Apis
{
    public static class ProductApi
    {
        public static IEndpointRouteBuilder MapProductApi(this IEndpointRouteBuilder builder)
        {
            var vApi = builder.NewVersionedApi("ecommerce");
            var v1 = vApi.MapGroup("api/v{version:apiVersion}/ecommerce").HasApiVersion(1, 0);

            v1.MapPost("/products", (IProductService productService, ProductCreateDto product) => productService.AddProductAsync(product));
            v1.MapGet("/products/{productId:guid}", async (IProductService service, Guid productId, [FromQuery] string? includeProperties = null) =>
            {
                var product = await service.GetProductByIdAsync(productId, includeProperties);
                return product is not null ? Results.Ok(product) : Results.NotFound();
            });
            v1.MapGet("/products", async (IProductService service, [FromQuery] string? includeProperties = null) =>
            {
                var products = await service.GetAllProductAsync(includeProperties);
                return Results.Ok(products);
            });
            v1.MapPut("/products/{productId:guid}", async (IProductService service, Guid productId, ProductUpdateDto dto) =>
            {
                if (productId != dto.Id)
                {
                    return Results.BadRequest("ID trong URL không khớp với body");
                }

                var updated = await service.UpdateProductAsync(productId, dto);
                return Results.Ok(updated);
            });
            v1.MapDelete("/products/{productId:guid}", async (IProductService service, Guid productId) =>
            {
                var success = await service.DeleteProductAsync(productId);
                return success ? Results.Ok() : Results.NotFound();
            });

            return builder;
        }
    }
}
