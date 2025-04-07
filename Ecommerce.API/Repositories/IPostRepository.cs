namespace Ecommerce.API.Repositories
{
    public interface IPostRepository
    {
        Task<Post> AddPostAsync(Post post);
        Task<Post?> GetPostByIdAsync(Guid postId, string? includeProperties = null);
        Task<Post> UpdatePostAsync(Post post);
        Task<bool> DeletePostAsync(Guid postId);
        Task<IEnumerable<Post>> GetPostsAsync(string? includeProperties = null);
    }
}
