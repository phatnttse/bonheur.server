using Bonheur.Services.Interfaces;
using Bonheur.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Bonheur.Services
{
    [Authorize]
    public sealed class NotificationHubService : Hub
    {
        private readonly INotificationService _notificationService;

        public NotificationHubService(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public override async Task OnConnectedAsync()
        {
            string userId = Utilities.GetCurrentUserId();

            if (!string.IsNullOrEmpty(userId))
            {
                await _notificationService.AddConnection(userId, Context.ConnectionId);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            string userId = Utilities.GetCurrentUserId();

            if (!string.IsNullOrEmpty(userId))
            {
                await _notificationService.RemoveConnection(userId);
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
