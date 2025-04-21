using Ecommerce.Infrastructure.Dtos;
using Ecommerce.Infrastructure.Models.Dtos;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using static Ecommerce.Infrastructure.Models.Dtos.ProductCreateDto;

using ProductQueryParameters = Ecommerce.Infrastructure.Models.ProductQueryParameters;

namespace Ecommerce.API.Apis
{
    public static class ProductApi
    {
        public static IEndpointRouteBuilder MapProductApi(this IEndpointRouteBuilder builder)
        {
            var vApi = builder.NewVersionedApi("ecommerce");
            var v1 = vApi.MapGroup("api/v{version:apiVersion}/ecommerce").HasApiVersion(1, 0);
            var v2 = vApi.MapGroup("api/v{version:apiVersion}/ecommerce").HasApiVersion(2, 0);

            v1.MapPost("/products", async (IProductService productService,
                                            [FromForm] ProductCreateDto productDto) =>
                                    {
                                        return await productService.AddProductAsync(productDto);
                                    })
                                    .Accepts<ProductCreateDto>("multipart/form-data")
                                    .DisableAntiforgery();

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
            v1.MapPut("/products/{productId:guid}", async (IProductService service, Guid productId, [FromForm] ProductUpdateDto dto) =>
            {
                if (productId != dto.Id)
                {
                    return Results.BadRequest("ID trong URL không khớp với body");
                }

                var updated = await service.UpdateProductAsync(productId, dto);
                return Results.Ok(updated);
            }).Accepts<ProductCreateDto>("multipart/form-data")
             .DisableAntiforgery();
            v1.MapDelete("/products/{productId:guid}", async (IProductService service, Guid productId) =>
            {
                var success = await service.DeleteProductAsync(productId);
                return success ? Results.Ok() : Results.NotFound();
            });

            //get san pham 
            v2.MapGet("/products", async ( [FromQuery] string? id,
                                           [FromQuery] string? slug,
                                           IProductService productService) =>
             {
                if (!string.IsNullOrEmpty(slug))
                {
                    var productBySlug = await productService.GetProductBySlugAsync(slug);
                    return productBySlug is null
                        ? Results.NotFound()
                        : Results.Ok(productBySlug);
                }

                if (Guid.TryParse(id, out var productId))
                {
                    var product = await productService.GetProductByIdAsync(productId);
                    return product is null ? Results.NotFound() : Results.Ok(product);
                }

                return Results.BadRequest("id san phẩm không hợp lệ");
            })
                 .WithName("GetProduct")
                 .Produces<ProductDto>(StatusCodes.Status200OK)
                 .Produces(StatusCodes.Status400BadRequest)
                 .Produces(StatusCodes.Status404NotFound);


            v1.MapPost("/test-upload", async (HttpRequest request) =>
            {
                var form = await request.ReadFormAsync();
                var files = form.Files;
                Console.WriteLine($"Received {files.Count} files");
                return Results.Ok(files.Select(f => f.FileName));
            }).DisableAntiforgery();

            v1.MapPost("/test", async (IFormFile file1) =>
            {
                if (file1 == null)
                {
                    return Results.BadRequest("No file uploaded");
                }

                Console.WriteLine($"File Name: {file1.FileName}, Length: {file1.Length}");
                return Results.Ok("OK");
            }).DisableAntiforgery();

            v1.MapGet("antiforgery/token", (IAntiforgery forgeryService, HttpContext context) =>
            {
                var tokens = forgeryService.GetAndStoreTokens(context);
                var xsrfToken = tokens.RequestToken!;
                return TypedResults.Content(xsrfToken, "text/plain");
            });

            v1.MapGet("/products/page", async (
                    IProductService service,
                    [AsParameters] ProductQueryParameters parameters,
                    CancellationToken cancellationToken = default
                ) =>
            {
                var paginatedResponse = await service.GetAllProductsPaginatedAsync(
                    parameters,
                    cancellationToken
                );
                return Results.Ok(paginatedResponse);
            });

            return builder;
        }
    }
}
