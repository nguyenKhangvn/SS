using Microsoft.EntityFrameworkCore;
using Ecommerce.API.Repositories.Interfaces;

namespace Ecommerce.API.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly EcommerceDbContext _context;

        public MessageRepository(EcommerceDbContext context)
        {
            _context = context;
        }

        public async Task<Message?> GetByIdAsync(Guid id)
        {
            return await _context.Messages
                .Include(m => m.Sender)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<Message>> GetMessagesForChatAsync(Guid chatId, int skip = 0, int take = 50)
        {
            return await _context.Messages
                .Include(m => m.Sender)
                .Where(m => m.ChatId == chatId)
                .OrderByDescending(m => m.SentAt)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<Message> AddAsync(Message message)
        {
            message.SentAt = DateTime.UtcNow;
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
            return message;
        }

        public async Task<bool> UpdateAsync(Message message)
        {
            try
            {
                _context.Messages.Update(message);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message == null)
                return false;

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetUnreadCountAsync(Guid chatId, Guid userId)
        {
            return await _context.Messages
                .CountAsync(m => m.ChatId == chatId && 
                               m.SenderId != userId 
                               );
        }

        public async Task<bool> MarkMessagesAsReadAsync(Guid chatId, Guid userId)
        {
            try
            {
                var unreadMessages = await _context.Messages
                    .Where(m => m.ChatId == chatId && 
                               m.SenderId != userId)
                    .ToListAsync();


                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Messages.AnyAsync(m => m.Id == id);
        }
    }
}
