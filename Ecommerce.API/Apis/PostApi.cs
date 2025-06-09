using Ecommerce.Infrastructure.Models.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Apis
{
    public static class PostApi
    {
        public static IEndpointRouteBuilder MapPostApi(this IEndpointRouteBuilder builder)
        {
            var vApi = builder.NewVersionedApi("ecommerce");
            var v1 = vApi.MapGroup("api/v{version:apiVersion}/ecommerce").HasApiVersion(1, 0).RequireAuthorization();

            v1.MapPost("/post", (IPostService postService, PostDto post) => postService.AddPostAsync(post));
            v1.MapGet("/post/{postId:guid}", async (IPostService service, Guid postId, [FromQuery] string? includeProperties = null) =>
            {
                var post = await service.GetPostByIdAsync(postId, includeProperties);
                return post is not null ? Results.Ok(post) : Results.NotFound();
            });
            v1.MapGet("/post", async (IPostService service, [FromQuery] string? includeProperties = null) =>
            {
                var posts = await service.GetPostsAsync(includeProperties);
                return Results.Ok(posts);
            });
            v1.MapPut("/post/{postId:guid}", async (IPostService service, Guid postId, PostDto dto) =>
            {
                if (postId != dto.Id)
                {
                    return Results.BadRequest("ID trong URL không khớp với body");
                }

                var updated = await service.UpdatePostAsync(dto);
                return Results.Ok(updated);
            });
            v1.MapDelete("/post/{postId:guid}", async (IPostService service, Guid postId) =>
            {
                var success = await service.DeletePostAsync(postId);
                return success ? Results.Ok() : Results.NotFound();
            });

            return builder;
        }
    }
}
