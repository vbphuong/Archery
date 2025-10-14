using Microsoft.AspNetCore.SignalR;

namespace Archery.Hubs
{
    public class InboxHub : Hub
    {
        public async Task SendMessage(string senderId, string receiverId, string message)
        {
            // Gửi cho người nhận
            await Clients.User(receiverId).SendAsync("ReceiveMessage", senderId, message, DateTime.UtcNow);
        }
    }
}
