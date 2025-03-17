using Bonheur.BusinessObjects.Entities;
using Bonheur.BusinessObjects.Models;
using Bonheur.Services.DTOs.Notification;
using Bonheur.Services.MessageBrokers.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services.Interfaces
{
    public interface INotificationService
    {
        Task SendNotificationToUser(string userId, NotificationCreatedEvent notification);
        Task AddConnection(string userId, string connectionId);
        Task RemoveConnection(string userId);
        Task BroadcastNotificationToAllUsers(NotificationCreatedEvent notification);
        Task<ApplicationResponse> CreateNotification(CreateNotificationDTO request);
        Task<ApplicationResponse> GetNotificationsByAccountAsync(int pageNumber, int pageSize);
    }
}
