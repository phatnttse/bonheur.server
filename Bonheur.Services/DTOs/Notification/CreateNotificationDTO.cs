using Bonheur.BusinessObjects.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services.DTOs.Notification
{
    public class CreateNotificationDTO
    {
        public string? Title { get; set; }

        public string? Content { get; set; }

        public string? RecipientId { get; set; }

        public NotificationType Type { get; set; }

        public string? Link { get; set; }
    }
}
