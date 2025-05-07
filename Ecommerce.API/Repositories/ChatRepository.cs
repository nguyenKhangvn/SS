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

        public async Task<Chat> AddAsync(Chat chat)
        {
            _context.Chats.AddAsync(chat); 
            await _context.SaveChangesAsync();
            return chat;
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
                    c.Participants.Any(p => p.UserId == customerId))
                .OrderByDescending(c => c.Messages.Max(m => m.SentAt)) 
                .FirstOrDefaultAsync();
        }

        //public async Task<Chat?> FindOrCreateDirectChatAsync(Guid userId1, Guid userId2)
        //{
        //    var existingChat = await _context.Chats
        //        .Include(c => c.Participants)
        //        .Where(c => c.Participants.Count == 2 ||
        //                   c.Participants.Any(p => p.UserId == userId1) ||
        //                   c.Participants.Any(p => p.UserId == userId2))
        //        .FirstOrDefaultAsync();

        //    if (existingChat != null)
        //    {
        //        return existingChat;
          //  }

            //// Create new chat
            //var chat = new Chat
            //{
            //    Title = $"Direct Chat",
            //    Status = ChatStatus.ACTIVE,
            //    CreatedAt = DateTime.UtcNow,
            //    UpdatedAt = DateTime.UtcNow,
            //    Participants = new List<ChatParticipant>
            //    {
            //        new ChatParticipant { UserId = userId1, JoinedAt = DateTime.UtcNow },
            //        new ChatParticipant { UserId = userId2, JoinedAt = DateTime.UtcNow }
            //    }
            //};

            //await _context.Chats.AddAsync(chat);
        //    await _context.SaveChangesAsync();
        //    return chat;
        //}

        public async Task<IEnumerable<ChatConversationDto>> GetUserConversationsAsync(Guid userId)
        {
            var chats = await _context.Chats
                .Include(c => c.Participants)
                    .ThenInclude(p => p.User)
                .Include(c => c.Messages)
                .ToListAsync();

            var conversations = new List<ChatConversationDto>();

            foreach (var chat in chats)
            {
                // Get other participant
                var otherParticipant = chat.Participants.FirstOrDefault(p => p.UserId != userId);
                if (otherParticipant?.User == null) continue;

                // Get last message
                var lastMessage = chat.Messages.OrderByDescending(m => m.SentAt).FirstOrDefault();

                conversations.Add(new ChatConversationDto
                {
                    UserId = otherParticipant.UserId,
                    UserName = otherParticipant.User.Name,
                    LastMessage = lastMessage?.Content ?? string.Empty,
                    LastMessageTime = lastMessage?.SentAt ?? DateTime.UtcNow,
                    UnreadCount = chat.Messages.Count(m => m.SenderId != userId && m.SentAt > DateTime.UtcNow.AddMinutes(-1))
                });
            }

            return conversations;
        }

        public async Task<int> GetUnreadCountAsync(Guid userId)
        {
            var chats = await _context.Chats
                .Include(c => c.Messages)
                .Where(c => c.Participants.Any(p => p.UserId == userId))
                .ToListAsync();

            return chats.Sum(chat => chat.Messages.Count(m =>
                m.SenderId != userId &&
                m.SentAt > DateTime.UtcNow.AddMinutes(-1)));
        }
    }
}