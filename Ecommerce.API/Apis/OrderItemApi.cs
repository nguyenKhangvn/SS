using Ecommerce.Infrastructure.Models.Dtos;

namespace Ecommerce.API.Apis
{
    public static class OrderItemApi
    {
        public static IEndpointRouteBuilder MapOrderItemApi(this IEndpointRouteBuilder builder)
        {
            var vApi = builder.NewVersionedApi("ecommerce");
            var v1 = vApi.MapGroup("api/v{version:apiVersion}/ecommerce").HasApiVersion(1, 0).RequireAuthorization();

            v1.MapPost("/order-item", (IOrderItemService orderItemService, OrderItemDto orderItem) => orderItemService.CreateAsync(orderItem));
            v1.MapGet("/order-item/{orderItemId:guid}", async (IOrderItemService service, Guid orderItemId, [FromQuery] string? includeProperties = null) =>
            {
                var orderItem = await service.GetByIdAsync(orderItemId);
                return orderItem is not null ? Results.Ok(orderItem) : Results.NotFound();
            });
            v1.MapGet("/order-item", async (IOrderItemService service, [FromQuery] string? includeProperties = null) =>
            {
                var orderItems = await service.GetAllAsync();
                return Results.Ok(orderItems);
            });
            v1.MapPut("/order-item/{orderItemId:guid}", async (IOrderItemService service, Guid orderItemId, OrderItemDto dto) =>
            {
                if (orderItemId != dto.Id)
                {
                    return Results.BadRequest("ID trong URL không khớp với body");
                }

                var updated = await service.UpdateAsync(orderItemId, dto);
                return Results.Ok(updated);
            });
            v1.MapDelete("/order-item/{orderItemId:guid}", async (IOrderItemService service, Guid orderItemId) =>
            {
                var success = await service.DeleteAsync(orderItemId);
                return success ? Results.Ok() : Results.NotFound();
            });

            return builder;
        }
    }
}
