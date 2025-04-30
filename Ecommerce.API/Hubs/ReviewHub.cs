// Hubs/ReviewHub.cs
using Microsoft.AspNetCore.SignalR;

namespace Ecommerce.API.Hubs
{
    public class ReviewHub : Hub
    {
        // Client sẽ tham gia nhóm theo productId
        public async Task JoinProductReviews(string productId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, productId);
        }

        // Client rời nhóm
        public async Task LeaveProductReviews(string productId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, productId);
        }
    }
}