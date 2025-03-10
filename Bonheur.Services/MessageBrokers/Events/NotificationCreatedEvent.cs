using Bonheur.BusinessObjects.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services.MessageBrokers.Events
{
    public record NotificationCreatedEvent
    {
        public int Id { get; init; }
        public string? Title { get; init; }
        public string? Content { get; init; }
        public string? RecipientId { get; init; }
        public bool IsRead { get; init; }
        public NotificationType Type { get; init; }
        public string? Link { get; init; }
        public DateTimeOffset CreatedAt { get; init; }
        public DateTimeOffset UpdatedAt { get; init; }

    }
}
