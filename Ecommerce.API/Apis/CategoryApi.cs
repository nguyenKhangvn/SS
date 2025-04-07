using Ecommerce.API.Services;
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

            v1.MapPost("/categorys", (ICategoryService categoryService, CreateCategoryDto category) => categoryService.AddCategory(category));
            v1.MapGet("/categorys/{categoryId:guid}", async (ICategoryService repo, Guid categoryId, [FromQuery] string? includeProperties = null) =>
            {
                var category = await repo.GetCategoryByIdAsync(categoryId, includeProperties);
                return category is not null ? Results.Ok(category) : Results.NotFound();
            });

            v1.MapPut("/categorys/{categoryId:guid}", async (ICategoryService categoryService, CreateCategoryDto category,Guid categoryId) =>
            {
                if(categoryId == category.Id)
                {
                    var updatedCategory = await categoryService.UpdateCategory(category);
                    return Results.Ok(updatedCategory);
                }
                return Results.BadRequest("Không tìm thấy");
            });

            v1.MapGet("/categorys", (ICategoryService categoryService) => categoryService.GetCategories());

            v1.MapDelete("/categorys/{categoryId:guid}", async (ICategoryService categoryService, Guid categoryId) =>
            {
                var result = await categoryService.DeleteCategory(categoryId);
                return result ? Results.NoContent() : Results.NotFound();
            });

            return builder;
        }
    }
}
