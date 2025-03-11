using Bonheur.BusinessObjects.Entities;
using Bonheur.BusinessObjects.Enums;
using Bonheur.BusinessObjects.Models;
using Bonheur.Repositories;
using Bonheur.Repositories.Interfaces;
using Bonheur.Services.DTOs.Notification;
using Bonheur.Services.DTOs.Order;
using Bonheur.Services.Interfaces;
using Bonheur.Services.MessageBrokers.Events;
using Bonheur.Utils;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;


namespace Bonheur.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private static readonly ConcurrentDictionary<string, string> _connections = new();
        private readonly IHubContext<NotificationHubService> _hubContext;
        private readonly IEventBus _eventBus;

        public NotificationService(
            INotificationRepository notificationRepository,
            IHubContext<NotificationHubService> hubContext,
            IEventBus eventBus
        )
        {
            _notificationRepository = notificationRepository;
            _hubContext = hubContext;
            _eventBus = eventBus;
        }

        public Task AddConnection(string userId, string connectionId)
        {
            _connections[userId] = connectionId;
            return Task.CompletedTask;
        }

        public Task RemoveConnection(string userId)
        {
            _connections.TryRemove(userId, out _);
            return Task.CompletedTask;
        }

        public async Task SendNotificationToUser(string userId, NotificationCreatedEvent notification)
        {
            string? connectionId = _connections.Values.FirstOrDefault(x => x == userId);

            if (connectionId != null)
            {
                await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveNotification", notification);
            }
        }

        public async Task BroadcastNotificationToAllUsers(NotificationCreatedEvent notification)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", notification);
        }

        public async Task<ApplicationResponse> CreateNotification(CreateNotificationDTO request)
        {
            try
            {
                if (request.Type == NotificationType.Broadcast && request.RecipientId == null)
                {
                     await this._eventBus.PublishAsync(request);
                }
                else 
                {
                    Notification notification = new Notification
                    {
                        Title = request.Title,
                        Content = request.Content,
                        RecipientId = request.RecipientId,
                        Type = request.Type,
                        Link = request.Link,
                        IsRead = false
                    };

                    await _notificationRepository.CreateNotificationAsync(notification);

                    string? connectionId = _connections.Values.FirstOrDefault(x => x == request.RecipientId);

                    if (connectionId != null)
                    {
                        await this._hubContext.Clients.Client(connectionId).SendAsync("ReceiveNotification", notification);
                    }
                }

                return new ApplicationResponse
                {
                    Data = null,
                    Message = "Notification created successfully",
                    Success = true,
                    StatusCode = System.Net.HttpStatusCode.OK
                };

            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApiException(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }
    }
}
