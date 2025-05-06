using Ecommerce.API.Repositories;
using Ecommerce.Infrastructure.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.API.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly EcommerceDbContext _context;

        public ChatRepository(EcommerceDbContext context)
        {
            _context = context;
        }

        public async Task<Chat?> GetByIdAsync(Guid id)
        {
            return await _context.Chats
                .Include(c => c.Messages)
                .Include(c => c.Participants)
                    .ThenInclude(cp => cp.User)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Chat>> GetChatsForUserAsync(Guid userId)
        {
            return await _context.ChatParticipants
                .Where(cp => cp.UserId == userId)
                .Select(cp => cp.Chat)
                .Include(c => c.Messages.OrderByDescending(m => m.SentAt).Take(1)) 
                .Include(c => c.Participants)
                    .ThenInclude(cp => cp.User)
                .OrderByDescending(c => c.Messages.Any() ? c.Messages.Max(m => m.SentAt) : c.CreatedAt) // Order by last message or creation date
                .ToListAsync();
        }

        public async Task AddAsync(Chat chat)
        {
            await _context.Chats.AddAsync(chat);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Chat chat)
        {
            _context.Chats.Update(chat);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var chat = await _context.Chats.FindAsync(id);
            if (chat != null)
            {
                _context.Chats.Remove(chat);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Chats.AnyAsync(c => c.Id == id);
        }

        public async Task<Chat?> FindChatByCustomerIdAsync(Guid customerId)
        {
            return await _context.Chats
                .Include(c => c.Participants)
                .ThenInclude(cp => cp.User)
                .Include(c => c.Messages)
                .Where(c =>
                    c.Participants.Count == 2 &&
                    c.Participants.Any(p => p.UserId == customerId))
                .OrderByDescending(c => c.Messages.Max(m => m.SentAt)) // Latest chat first
                .FirstOrDefaultAsync();
        }

    }
}