using Ecommerce.Infrastructure.Models.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Apis
{
    public static class CouponApi
    {
        public static IEndpointRouteBuilder MapCouponApi(this IEndpointRouteBuilder builder)
        {
            var vApi = builder.NewVersionedApi("ecommerce");
            var v1 = vApi.MapGroup("api/v{version:apiVersion}/ecommerce").HasApiVersion(1, 0);

            // [POST] http://localhost:5000/api/v1/ecommerce/coupons
            v1.MapPost("/coupons", async (ICouponService couponService, CouponCreateDto coupon) =>
            {
                var created = await couponService.AddAsync(coupon);
                return created != null ? Results.Ok(created) : Results.BadRequest("Không thể tạo coupon.");
            });

            // [GET] http://localhost:5000/api/v1/ecommerce/coupons/{couponId:guid}
            v1.MapGet("/coupons/{couponId:guid}", async (ICouponService service, Guid couponId) =>
            {
                var coupon = await service.GetByIdAsync(couponId);
                return coupon != null ? Results.Ok(coupon) : Results.NotFound();
            });

            // [GET] http://localhost:5000/api/v1/ecommerce/coupons
            v1.MapGet("/coupons", async (ICouponService service, [FromQuery] Guid? userId, [FromQuery] bool onlyActive = true) =>
            {
                var coupons = await service.GetAllAsync(userId, onlyActive);
                return Results.Ok(coupons);
            });

            // [PUT] http://localhost:5000/api/v1/ecommerce/coupons/{couponId:guid}
            v1.MapPut("/coupons/{couponId:guid}", async (ICouponService service, Guid couponId, CouponCreateDto dto) =>
            {
                if (couponId != dto.Id)
                {
                    return Results.BadRequest("ID trong URL không khớp với body");
                }

                var updated = await service.UpdateAsync(couponId, dto);
                return updated != null ? Results.Ok(updated) : Results.NotFound();
            });

            // [DELETE] http://localhost:5000/api/v1/ecommerce/coupons/{couponId:guid}
            v1.MapDelete("/coupons/{couponId:guid}", async (ICouponService service, Guid couponId) =>
            {
                var success = await service.DeleteAsync(couponId);
                return success ? Results.Ok() : Results.NotFound();
            });

            // [POST] http://localhost:5000/api/v1/ecommerce/coupons/apply
            v1.MapPost("/coupons/apply", async (ICouponService service, ApplyCouponRequest request) =>
            {
                var result = await service.ApplyCouponAsync(request.Code, request.UserId, request.OrderTotal);
                return Results.Ok(result);
            });

            // [POST] http://localhost:5000/api/v1/ecommerce/coupons/save
            v1.MapPost("/coupons/save", async (ICouponService service, SaveCouponRequest request) =>
            {
                var result = await service.SaveCouponAsync(request.UserId, request.CouponCode);
                return result.Success ? Results.Ok(result) : Results.BadRequest(result);
            });

            // [GET] http://localhost:5000/api/v1/ecommerce/coupons/saved
            v1.MapGet("/coupons/saved", async (ICouponService service, Guid userId) =>
            {
                var savedCoupons = await service.GetSavedCouponsAsync(userId);
                return Results.Ok(savedCoupons);
            });

            return builder;
        }
    }
}