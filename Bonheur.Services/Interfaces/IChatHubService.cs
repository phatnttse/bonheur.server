using Bonheur.Services.DTOs.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services.Interfaces
{
    public interface IChatHubService
    {
        Task OnConnectedAsync();
        Task OnDisconnectedAsync(Exception? exception);
        Task LoadMessages(string recipientId, int pageNumber = 1);
        Task SendMessage(MessageRequestDTO message);
        Task NotifyTyping(string recipientId);

    }
}
