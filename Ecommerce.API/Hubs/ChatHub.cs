// Hubs/ChatHub.cs
using Microsoft.AspNetCore.SignalR;

namespace Ecommerce.API.Hubs
{
    public class ChatHub : Hub
    {
        // Client sẽ tham gia nhóm theo chatId (dùng chung cho cả khách hàng và nhân viên)
        public async Task JoinChat(string chatId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatId);
        }

        // Client rời nhóm chat
        public async Task LeaveChat(string chatId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId);
        }

        // Gửi tin nhắn trong nhóm chat
        public async Task SendMessage(string chatId, string message)
        {
            // Gửi tin nhắn đến tất cả những người trong cùng nhóm chat
            await Clients.Group(chatId).SendAsync("ReceiveMessage", message);
        }
    }
}
