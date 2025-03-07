using Bonheur.BusinessObjects.Entities;
using Bonheur.BusinessObjects.Enums;
using Bonheur.Repositories.Interfaces;
using Bonheur.Services.Interfaces;
using Bonheur.Services.MessageBrokers.Events;


namespace Bonheur.Services
{
    public class NotificationService : INotificationService
    {
        //private readonly IEventBus _eventBus;
        private readonly INotificationRepository _notificationRepository;

        public NotificationService(
            //IEventBus eventBus,
            INotificationRepository notificationRepository
        )
        {
            //_eventBus = eventBus;
            _notificationRepository = notificationRepository;
        }

        public async Task<Notification> CreateNotificationAsync()
        {
            Notification notification = new Notification
            {
                Title = "Notification Title",
                Content = "Notification Message",
                RecipientId = "RecipientId",
                IsRead = false,
                Type = NotificationType.General,
                Link = "https://bonheur.pro"
            };

            await _notificationRepository.CreateNotificationAsync(notification);

            NotificationCreatedEvent notificationEvent = new NotificationCreatedEvent
            {
                Id = notification.Id,
                Title = "Notification Title",
                Content = "Notification Message",
                RecipientId = "RecipientId",
                IsRead = false,
                Type = NotificationType.General,
                Link = "https://bonheur.pro"
            };

            //await _eventBus.PublishAsync(notificationEvent);

            return notification;
        }
    }
}
