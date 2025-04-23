using Ecommerce.Infrastructure.Models.Dtos;

namespace Ecommerce.API.Apis
{
    public static class ProductStoreInventoryApi
    {
        public static IEndpointRouteBuilder MapProductStoreInventoryApi(this IEndpointRouteBuilder builder)
        {
            var vApi = builder.NewVersionedApi("ecommerce");
            var v1 = vApi.MapGroup("api/v{version:apiVersion}/ecommerce").HasApiVersion(1, 0);

            // Thêm mới inventory
            v1.MapPost("/product-store-inventory", async (IProductStoreInventoryService service, AddOrUpdateProductStoreInventoryDto dto) =>
            {
                var inventory = await service.AddAsync(dto);
                return Results.Created($"/api/v1/ecommerce/product-store-inventory/{inventory.Id}", inventory);
            });

            // Lấy inventory theo ID
            v1.MapGet("/product-store-inventory/{id:guid}", async (IProductStoreInventoryService service, Guid id) =>
            {
                var inventory = await service.GetByIdAsync(id);
                return Results.Ok(inventory);

            });

            // Lấy tất cả inventory của một sản phẩm
            v1.MapGet("/product-store-inventory/product/{productId:guid}", async (IProductStoreInventoryService service, Guid productId) =>
            {
                var inventories = await service.GetByProductIdAsync(productId);
                return Results.Ok(inventories);
            });

            // Lấy tất cả inventory của một cửa hàng
            v1.MapGet("/product-store-inventory/store/{storeId:guid}", async (IProductStoreInventoryService service, Guid storeId) =>
            {
                var inventories = await service.GetByStoreIdAsync(storeId);
                return Results.Ok(inventories);
            });

            // Lấy inventory cụ thể của sản phẩm tại cửa hàng
            v1.MapGet("/product-store-inventory/product/{productId:guid}/store/{storeId:guid}", async (IProductStoreInventoryService service, Guid productId, Guid storeId) =>
            {
                var inventory = await service.GetByProductAndStoreAsync(productId, storeId);
                return inventory is not null ? Results.Ok(inventory) : Results.NotFound();
            });

            // Cập nhật inventory
            v1.MapPut("/product-store-inventory/{id:guid}", async (IProductStoreInventoryService service, Guid id, AddOrUpdateProductStoreInventoryDto dto) =>
            {

                var updated = await service.UpdateAsync(id, dto);
                return Results.Ok(updated);

            });

            // Cập nhật số lượng tồn kho (tăng/giảm)
            v1.MapPatch("/product-store-inventory/update-quantity", async (
                IProductStoreInventoryService service,
                [FromQuery] Guid productId,
                [FromQuery] Guid storeId,
                [FromQuery] int quantityChange) =>
            {

                await service.UpdateQuantityAsync(productId, storeId, quantityChange);
                return Results.NoContent();

            });

            // Xóa inventory
            v1.MapDelete("/product-store-inventory/{id:guid}", async (IProductStoreInventoryService service, Guid id) =>
            {

                await service.DeleteAsync(id);
                return Results.NoContent();
            });

            return builder;
        }
    }
}