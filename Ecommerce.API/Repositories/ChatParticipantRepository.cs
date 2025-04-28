using Ecommerce.API.Repositories.Interfaces;

namespace Ecommerce.API.Repositories
{
    public class ChatParticipantRepository : IChatParticipantRepository
    {
        private readonly EcommerceDbContext _context;

        public ChatParticipantRepository(EcommerceDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(ChatParticipant participant)
        {
            await _context.ChatParticipants.AddAsync(participant);
            await _context.SaveChangesAsync();
        }

        public async Task<ChatParticipant?> GetByChatAndUserAsync(Guid chatId, Guid userId)
        {
            return await _context.ChatParticipants
                .FirstOrDefaultAsync(cp => cp.ChatId == chatId && cp.UserId == userId);
        }

        public async Task<IEnumerable<ChatParticipant>> GetParticipantsForChatAsync(Guid chatId)
        {
            return await _context.ChatParticipants
                .Where(cp => cp.ChatId == chatId)
                .Include(cp => cp.User) // Include User info
                .ToListAsync();
        }

        public async Task<bool> IsUserInChatAsync(Guid chatId, Guid userId)
        {
            return await _context.ChatParticipants.AnyAsync(cp => cp.ChatId == chatId && cp.UserId == userId);
        }
    }
}
