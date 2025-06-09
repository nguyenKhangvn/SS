using Ecommerce.Infrastructure.Models.Dtos;

namespace Ecommerce.API.Apis
{
    public static class DiscountApi
    {
        public static IEndpointRouteBuilder MapDiscountApi(this IEndpointRouteBuilder builder)
        {
            var vApi = builder.NewVersionedApi("ecommerce");
            var v1 = vApi.MapGroup("api/v{version:apiVersion}/ecommerce").HasApiVersion(1, 0).RequireAuthorization();

            v1.MapGet("/discount", async (IDiscountService service) =>
            {
                return Results.Ok(await service.GetAllAsync());
            });

            v1.MapGet("/discount/{id:guid}", async (Guid id, IDiscountService service) =>
            {
                var result = await service.GetByIdAsync(id);
                return result == null ? Results.NotFound() : Results.Ok(result);
            });

            v1.MapPost("/discount", async (DiscountDto dto, IDiscountService service) =>
            {
                var result = await service.CreateAsync(dto);
                return Results.Created($"/api/discounts/{result.Id}", result);
            });

            v1.MapPut("/discount/{id:guid}", async (Guid id, DiscountDto dto, IDiscountService service) =>
            {
                var updated = await service.UpdateAsync(id, dto);
                return updated ? Results.NoContent() : Results.NotFound();
            });

            v1.MapDelete("/discount/{id:guid}", async (Guid id, IDiscountService service) =>
            {
                var deleted = await service.DeleteAsync(id);
                return deleted ? Results.NoContent() : Results.NotFound();
            });
            return builder;
        }
    }
}
