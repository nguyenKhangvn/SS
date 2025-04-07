using Ecommerce.Infrastructure.Models.Dtos;

namespace Ecommerce.API.Services.Interfaces
{
    public interface IPostService
    {
        Task<PostDto> AddPostAsync(PostDto post);
        Task<PostDto?> GetPostByIdAsync(Guid postId, string? includeProperties = null);
        Task<PostDto> UpdatePostAsync(PostDto post);
        Task<bool> DeletePostAsync(Guid postId);
        Task<IEnumerable<PostDto>> GetPostsAsync(string? includeProperties = null);
    }
}
