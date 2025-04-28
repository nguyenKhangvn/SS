using Ecommerce.Infrastructure.Models.Dtos;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Ecommerce.API.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IChatService _chatService;

        public ChatHub(IChatService chatService)
        {
            _chatService = chatService;
        }

        // Method for clients to send a message
        // This method will be called by the client via the SignalR connection
        public async Task SendMessage(SendMessageRequest request)
        {
            // Get the sender's User ID from the authenticated user's claims
            // You need to ensure your application authenticates users and sets the NameIdentifier claim
            var senderIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier);
            if (senderIdClaim == null || !Guid.TryParse(senderIdClaim.Value, out Guid senderUserId))
            {
                // Handle unauthenticated user or invalid user ID
                await Clients.Caller.SendAsync("ReceiveMessageError", "User not authenticated or user ID invalid.");
                return;
            }

            // Call the service layer to handle sending the message
            var messageDto = await _chatService.SendMessageAsync(request, senderUserId);

            if (messageDto != null)
            {
                // If message was sent successfully, broadcast it to all participants of the chat
                // We use the chat ID as the group name
                await Clients.Group(request.ChatId.ToString()).SendAsync("ReceiveMessage", messageDto);
            }
            else
            {
                // Handle cases where message sending failed (e.g., chat not found, user not participant)
                await Clients.Caller.SendAsync("ReceiveMessageError", "Failed to send message.");
            }
        }

        // Method for clients to join a specific chat group
        // This should be called when a user opens a chat window/view
        public async Task JoinChatGroup(Guid chatId)
        {
            // Get the user ID (same logic as SendMessage)
            var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                await Clients.Caller.SendAsync("JoinChatError", "User not authenticated or user ID invalid.");
                return;
            }

            // Optional: Verify if the user is actually a participant of this chat
            var isParticipant = await _chatService.GetChatByIdAsync(chatId); // You might need a dedicated service method to check participation
            if (isParticipant == null || !isParticipant.Participants.Any(p => p.UserId == userId))
            {
                await Clients.Caller.SendAsync("JoinChatError", "You are not a participant of this chat.");
                return;
            }


            // Add the current connection to the chat group
            await Groups.AddToGroupAsync(Context.ConnectionId, chatId.ToString());

            // Optional: Notify other participants that a user has joined (if needed)
            // await Clients.Group(chatId.ToString()).SendAsync("UserJoined", userId);
        }

        // Method for clients to leave a specific chat group
        // This should be called when a user closes a chat window/view
        public async Task LeaveChatGroup(Guid chatId)
        {
            // Remove the current connection from the chat group
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId.ToString());

            // Optional: Notify other participants that a user has left (if needed)
            // await Clients.Group(chatId.ToString()).SendAsync("UserLeft", Context.UserIdentifier); // UserIdentifier is the NameIdentifier claim by default
        }

        // Override OnConnectedAsync to add the user to their relevant chat groups upon connection
        // This might be complex as you'd need to know which chats the user belongs to.
        // An alternative is to rely solely on the client calling JoinChatGroup.
        // For simplicity, we'll rely on the client calling JoinChatGroup after connecting.
        public override async Task OnConnectedAsync()
        {
            // You could potentially fetch the user's chats here and add them to groups,
            // but it requires authentication and potentially loading data on connection,
            // which might not be ideal depending on your application's scale.
            // For now, we'll assume the client will explicitly join groups.

            await base.OnConnectedAsync();
        }

        // Override OnDisconnectedAsync to handle disconnections
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            // SignalR automatically removes the connection from groups when it disconnects.
            // You might add logging or other cleanup here if needed.

            await base.OnDisconnectedAsync(exception);
        }
    }
} 