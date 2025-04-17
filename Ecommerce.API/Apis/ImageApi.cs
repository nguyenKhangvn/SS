namespace Ecommerce.API.Apis
{
    public static class ImageApi
    {
        public static IEndpointRouteBuilder MapImageApi(this IEndpointRouteBuilder builder)
        {
            var vApi = builder.NewVersionedApi("ecommerce");
            var v1 = vApi.MapGroup("api/v{version:apiVersion}/ecommerce").HasApiVersion(1, 0);

            //[GET] http://localhost:5000/api/v1/ecommerce/image/
            v1.MapGet("/image", async ([FromServices] IImageService service) =>
            {
                return Results.Ok(await service.GetAllImagesAsync());
            });

            return builder;
        }
    }
}
