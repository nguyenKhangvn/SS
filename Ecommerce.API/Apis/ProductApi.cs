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
            //update
            v1.MapPost("/products/{productId:guid}", async (IProductService service, 
                                                            Guid productId, 
                                                            [FromForm] ProductCreateDto dto) =>
            {

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

            v1.MapGet("/products/page", static async (HttpContext context,
                 IProductService service,
                 CancellationToken cancellationToken = default
             ) =>
            {
                foreach (var param in context.Request.Query)
                {
                    Console.WriteLine($"Parameter: {param.Key}, Value: {param.Value}");
                }

                var parameters = new ProductQueryParameters(
                    pageSize: context.Request.Query.TryGetValue("pageSize", out var ps) ? int.Parse(ps) : 10,
                    pageIndex: context.Request.Query.TryGetValue("pageIndex", out var pi) ? int.Parse(pi) : 1
                )
                {
                    CategoryId = context.Request.Query.TryGetValue("CategoryId", out var catId) ? Guid.Parse(catId) : null,
                    SearchTerm = context.Request.Query.TryGetValue("SearchTerm", out var search) ? search.ToString() : null,
                    Include = context.Request.Query.TryGetValue("Include", out var include) ? include.ToString() : null,
                    MinPrice = context.Request.Query.TryGetValue("MinPrice", out var min) && !string.IsNullOrEmpty(min) ? decimal.Parse(min) : null,
                    MaxPrice = context.Request.Query.TryGetValue("MaxPrice", out var max) && !string.IsNullOrEmpty(max) ? decimal.Parse(max) : null,
                    SortBy = context.Request.Query.TryGetValue("SortBy", out var sort) ? sort.ToString() : null,
                    SortOrder = context.Request.Query.TryGetValue("SortOrder", out var order) ? order.ToString() : "asc",
                    IsActive = context.Request.Query.TryGetValue("IsActive", out var active) ? bool.Parse(active) : null
                };

                var paginatedResponse = await service.GetAllProductsPaginatedAsync(
                    parameters,
                    cancellationToken
                );

                return Results.Ok(paginatedResponse);
            });
            //update product
            v2.MapPost("/products/{productId:guid}", async (IProductService service,
                                                           Guid productId,
                                                           [FromForm] ProductCreateDto dto) =>
            {

                var updated = await service.UpdateProductAsyncToCloud(productId, dto);
                return Results.Ok(updated);
            }).Accepts<ProductCreateDto>("multipart/form-data")
            .DisableAntiforgery();
            //add product to cloud
            v2.MapPost("/products", async (IProductService productService,
                                            [FromForm] ProductCreateDto productDto) =>
            {
                return await productService.AddProductAsyncToCloud(productDto);
            })
                                    .Accepts<ProductCreateDto>("multipart/form-data")
                                    .DisableAntiforgery();
            //gợi ý sp
            v1.MapGet("/recommended", async (IProductService service, [FromQuery] int topN = 4) =>
            {
                var products = await service.GetRecommendedProductsAsync(topN);
                return Results.Ok(products);
            })
            .Produces<List<ProductDto>>(StatusCodes.Status200OK);

            v1.MapPost("/track-click", async (IProductService service, [FromBody] TrackClickDto request) =>
            {
                if (request.ProductId == Guid.Empty)
                {
                    return Results.BadRequest(" productId k hợp lệ");
                }
                await service.IncrementClickCountAsync(request.ProductId);
                return Results.Ok();
            })
            .WithName("TrackClick")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);

            return builder;
        }
    }
}
