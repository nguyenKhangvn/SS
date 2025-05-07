using Microsoft.EntityFrameworkCore;

namespace Ecommerce.API.Repositories
{
    public class PostRepository : IPostRepository
    {
        private EcommerceDbContext _context;
        public PostRepository(EcommerceDbContext context)
        {
            _context = context;
        }

        public async Task<Post> AddPostAsync(Post post)
        {
            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();
            return post;
        }

        public async Task<bool> DeletePostAsync(Guid postId)
        {
            var post = await _context.Posts.FindAsync(postId);
            if (post == null)
            {
                return false;
            }
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Post?> GetPostByIdAsync(Guid postId, string? includeProperties = null)
        {
            var query = _context.Posts.AsQueryable();
            if (!string.IsNullOrEmpty(includeProperties))
            {
                // Thêm các thuộc tính liên kết vào query (Eager Loading)
                query = query.Include(includeProperties);
            }
            return await query.FirstOrDefaultAsync(p => p.Id == postId);
        }

        public Task<IEnumerable<Post>> GetPostsAsync(string? includeProperties = null)
        {
            var query = _context.Posts.AsQueryable();
            if (!string.IsNullOrEmpty(includeProperties))
            {
                // Thêm các thuộc tính liên kết vào query (Eager Loading)
                query = query.Include(includeProperties);
            }
            return Task.FromResult(query.AsEnumerable());
        }

        public async Task<Post> UpdatePostAsync(Post post)
        {
            _context.Posts.Update(post);
            await _context.SaveChangesAsync();
            return post;
        }
    } 
}
