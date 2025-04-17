using Ecommerce.Infrastructure.Models.Dtos;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Ecommerce.API.Apis
{
    public static class OrderApi
    {
        public static IEndpointRouteBuilder MapOrderApi(this IEndpointRouteBuilder builder)
        {
            var vApi = builder.NewVersionedApi("ecommerce");
            var v1 = vApi.MapGroup("api/v{version:apiVersion}/ecommerce").HasApiVersion(1, 0);

            v1.MapPost("/order", (IOrderService orderService, OrderDto order) => orderService.CreateAsync(order));
            v1.MapGet("/order/{orderId:guid}", async (IOrderService service, Guid orderId) =>
            {
                var order = await service.GetByIdAsync(orderId);
                return order is not null ? Results.Ok(order) : Results.NotFound();
            });
            v1.MapGet("/order", async (IOrderService service) =>
            {
                var orders = await service.GetAllAsync();
                return Results.Ok(orders);
            });
            v1.MapPut("/order/{orderId:guid}", async (IOrderService service, Guid orderId, OrderDto dto) =>
            {
                if (orderId != dto.Id)
                {
                    return Results.BadRequest("ID trong URL không khớp với body");
                }
                var updated = await service.UpdateAsync(dto);
                return Results.Ok(updated);
            });
            v1.MapDelete("/order/{orderId:guid}", async (IOrderService service, Guid orderId) =>
            {
                var success = await service.DeleteAsync(orderId);
                return success ? Results.Ok() : Results.NotFound();
            });

            v1.MapPut("/order/{orderId:guid}/status", async (IOrderService service, Guid orderId, [FromBody] UpdateOrderStatusDto dto) =>
            {
                var result = await service.UpdateStatusAsync(orderId, dto.Status);
                if (result == null)
                {
                    return Results.NotFound();
                }
                return Results.Ok();
            });

            return builder;
        }
    }
}
