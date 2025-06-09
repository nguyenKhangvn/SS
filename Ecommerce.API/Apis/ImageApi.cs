using Ecommerce.Infrastructure.Models.Dtos;

namespace Ecommerce.API.Apis
{
    public static class ImageApi
    {
        public static IEndpointRouteBuilder MapImageApi(this IEndpointRouteBuilder builder)
        {
            var vApi = builder.NewVersionedApi("ecommerce");
            var v1 = vApi.MapGroup("api/v{version:apiVersion}/ecommerce").HasApiVersion(1, 0).RequireAuthorization();

            //[GET] http://localhost:5000/api/v1/ecommerce/image/
            v1.MapGet("/image", async ([FromServices] IImageService service) =>
            {
                return Results.Ok(await service.GetAllImagesAsync());
            });
            // [GET] http://localhost:5000/api/v1/ecommerce/image/{id}
            v1.MapGet("/image/{id:guid}", async ([FromServices] IImageService service, Guid id) =>
            {
                var image = await service.GetImageByIdAsync(id);
                return image != null ? Results.Ok(image) : Results.NotFound();
            });

            // [GET] http://localhost:5000/api/v1/ecommerce/image/product/{productId}
            v1.MapGet("/image/product/{productId:guid}", async ([FromServices] IImageService service, Guid productId) =>
            {
                var images = await service.GetImagesByProductIdAsync(productId);
                return Results.Ok(images);
            });

            // [POST] http://localhost:5000/api/v1/ecommerce/image/upload
            v1.MapPost("/image/upload", async (
                [FromServices] IImageService service,
                [FromServices] IHostEnvironment env,
                IFormFile file,
                [FromQuery] Guid? productId) =>
            {
                if (file == null || file.Length == 0)
                    return Results.BadRequest("No file uploaded.");

                try
                {
                    // Kiểm tra định dạng file
                    var allowedImageTypes = new[] { "image/jpeg", "image/png", "image/gif" };
                    var allowedVideoTypes = new[] { "video/mp4", "video/mpeg", "video/webm" };
                    var isImage = allowedImageTypes.Contains(file.ContentType);
                    var isVideo = allowedVideoTypes.Contains(file.ContentType);

                    if (!isImage && !isVideo)
                        return Results.BadRequest("Only JPEG, PNG, GIF images or MP4, MPEG, WebM videos are allowed.");

                    // Kiểm tra kích thước file (tối đa 10MB)
                    if (file.Length > 10 * 1024 * 1024)
                        return Results.BadRequest("File size must be less than 10MB.");

                    // Tạo tên file duy nhất
                    var fileExtension = Path.GetExtension(file.FileName).ToLower();
                    var fileName = $"{Guid.NewGuid()}{fileExtension}";
                    var uploadsFolder = Path.Combine(env.ContentRootPath, "wwwroot", "images");

                    // Tạo thư mục uploads nếu chưa có
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var filePath = Path.Combine(uploadsFolder, fileName);

                    // Lưu file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // Tạo URL
                    var request = builder.ServiceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext?.Request;
                    var fileUrl = $"{request?.Scheme}://{request?.Host}/images/{fileName}";

                    // Lưu thông tin vào database
                    var image = new ImageDto
                    {
                        Id = Guid.NewGuid(),
                        ProductId = productId ?? Guid.Empty, // Nếu không có productId, để trống
                        Url = fileUrl,
                    };

                    var savedImage = await service.AddImageAsync(image);

                    return Results.Ok(new { Url = savedImage.Url });
                }
                catch (Exception ex)
                {
                    return Results.Problem($"Error uploading file: {ex.Message}");
                }
            }).Accepts<IFormFile>("multipart/form-data");

            // [DELETE] http://localhost:5000/api/v1/ecommerce/image/{id}
            v1.MapDelete("/image/{id:guid}", async ([FromServices] IImageService service, Guid id) =>
            {
                var deleted = await service.DeleteImageAsync(id);
                return deleted ? Results.Ok() : Results.NotFound();
            });

            // [PUT] http://localhost:5000/api/v1/ecommerce/image/{id}
            //v1.MapPut("/image/{id:guid}", async (
            //    [FromServices] IImageService service,
            //    Guid id,
            //    [FromBody] ImageDto image) =>
            //{
            //    if (id != image.Id)
            //        return Results.BadRequest("ID mismatch.");

            //    var updatedImage = await service.UpdateImageAsync(image);
            //    return updatedImage != null ? Results.Ok(updatedImage) : Results.NotFound();
            //});
            return builder;
        }
    }
}
