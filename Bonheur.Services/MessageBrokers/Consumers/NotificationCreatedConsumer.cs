using Bonheur.BusinessObjects.Entities;
using Bonheur.Repositories.Interfaces;
using Bonheur.Services.DTOs.Notification;
using Bonheur.Services.Interfaces;
using Bonheur.Services.MessageBrokers.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services.MessageBrokers.Consumers
{
    public sealed class NotificationCreatedConsumer : IConsumer<NotificationCreatedEvent>
    {
        private ILogger<NotificationCreatedConsumer> _logger;
        private readonly INotificationService _notificationService;
        private readonly INotificationRepository _notificationRepository;


        public NotificationCreatedConsumer(ILogger<NotificationCreatedConsumer> logger, INotificationService notificationService, INotificationRepository notificationRepository)
        {
            _logger = logger;
            _notificationService = notificationService;
            _notificationRepository = notificationRepository;
        }

        public async Task Consume(ConsumeContext<NotificationCreatedEvent> context)
        {
            var message = context.Message;

            _logger.LogInformation($"Notification created: {message}");

            await _notificationService.SendNotificationToUser(message.RecipientId!, message);
            
        }

    }
}
