using Ecommerce.Infrastructure.Models.Dtos;

namespace Ecommerce.API.Apis
{
    public static class CouponApi
    {
        public static IEndpointRouteBuilder MapCouponAPi(this IEndpointRouteBuilder builder)
        {
            var vApi = builder.NewVersionedApi("ecommerce");
            var v1 = vApi.MapGroup("api/v{version:apiVersion}/ecommerce").HasApiVersion(1, 0);
            v1.MapPost("/coupons", (ICouponService couponService, CouponDto coupon) => couponService.AddAsync(coupon));
            v1.MapGet("/coupons/{couponId:guid}", async (ICouponService couponService, Guid couponId) =>
            {
                var coupon = await couponService.GetByIdAsync(couponId);
                return coupon is not null ? Results.Ok(coupon) : Results.NotFound();
            });
            v1.MapGet("/coupons", async (ICouponService couponService) =>
            {
                var coupons = await couponService.GetAllAsync();
                return Results.Ok(coupons);
            });
            v1.MapPut("/coupons/{couponId:guid}", async (ICouponService couponService, Guid couponId, CouponDto dto) =>
            {
                if (couponId != dto.Id)
                {
                    return Results.BadRequest("ID trong URL không khớp với body");
                }
                var updated = await couponService.UpdateAsync(dto);
                return Results.Ok(updated);
            });
            v1.MapDelete("/coupons/{couponId:guid}", async (ICouponService couponService, Guid couponId) =>
            {
                var success = await couponService.DeleteAsync(couponId);
                return success ? Results.Ok() : Results.NotFound();
            });
            return builder;
        }
    }
}
