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

        public NotificationCreatedConsumer(ILogger<NotificationCreatedConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<NotificationCreatedEvent> context)
        {
            var message = context.Message;
            _logger.LogInformation($"Notification created: {message}");
            return Task.CompletedTask;
        }
    }
}
