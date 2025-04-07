using Ecommerce.API.Services.Interfaces;
using Ecommerce.Infrastructure.Models.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Apis
{
    public static class CategoryApi
    {
        public static IEndpointRouteBuilder MapCategoryApi(this IEndpointRouteBuilder builder)
        {
            var vApi = builder.NewVersionedApi("ecommerce");
            var v1 = vApi.MapGroup("api/v{version:apiVersion}/ecommerce").HasApiVersion(1, 0);

            v1.MapPost("/categorys", (ICategoryService categoryService, CategoryDto category) => categoryService.AddCategory(category));
            v1.MapGet("/{categoryId:guid}", async (ICategoryService repo, Guid categoryId, [FromQuery] string? includeProperties = null) =>
            {
                var category = await repo.GetCategoryById(categoryId, includeProperties);
                return category is not null ? Results.Ok(category) : Results.NotFound();
            });

            return builder;
        }
    }
}
