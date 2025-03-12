using Bonheur.BusinessObjects.Entities;
using Bonheur.BusinessObjects.Enums;
using Bonheur.Repositories.Interfaces;
using Bonheur.Services.DTOs.Notification;
using Bonheur.Services.Interfaces;
using Bonheur.Services.MessageBrokers.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Bonheur.Services.MessageBrokers.Consumers
{
    public class CreateBroadcastNotificationConsumer : IConsumer<CreateNotificationDTO>
    {
        private ILogger<NotificationCreatedConsumer> _logger;
        private readonly INotificationService _notificationService;
        private readonly INotificationRepository _notificationRepository;
        private readonly IUserAccountRepository _userAccountRepository;


        public CreateBroadcastNotificationConsumer(ILogger<NotificationCreatedConsumer> logger, INotificationService notificationService, INotificationRepository notificationRepository, IUserAccountRepository userAccountRepository)
        {
            _logger = logger;
            _notificationService = notificationService;
            _notificationRepository = notificationRepository;
            _userAccountRepository = userAccountRepository;
        }

        public async Task Consume(ConsumeContext<CreateNotificationDTO> context)
        {
            var message = context.Message;

            _logger.LogInformation($"Notification created: {message}");

            if (message.Type == NotificationType.Broadcast)
            {
                NotificationCreatedEvent notification = new NotificationCreatedEvent
                {
                    Title = message.Title,
                    Content = message.Content,
                    Type = message.Type,
                    Link = message.Link,
                    IsRead = false
                };

                 await _notificationService.BroadcastNotificationToAllUsers(notification);

                var allUsers = await _userAccountRepository.GetAllUsersAsync(); 

                var notifications = allUsers.Select(user => new Notification
                {
                    Title = message.Title,
                    Content = message.Content,
                    RecipientId = user.Id,
                    Type = NotificationType.Broadcast,
                    Link = message.Link,
                    IsRead = false
                }).ToList();

                await _notificationRepository.AddRangeAsync(notifications); 
            }
        }
    }
}
