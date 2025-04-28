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
        public async Task<IEnumerable<Message>> GetMessagesForChatAsync(Guid chatId, int skip = 0, int take = 50)
        {
            return await _context.Messages
                .Where(m => m.ChatId == chatId)
                .OrderBy(m => m.SentAt) // Order by time ascending
                .Skip(skip)
                .Take(take)
                .Include(m => m.Sender) // Include Sender info
                .ToListAsync();
        }

        public async Task AddAsync(Message message)
        {
            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Messages.AnyAsync(m => m.Id == id);
        }
    }
}
