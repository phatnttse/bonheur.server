using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services.DTOs.Account
{
    public class OnlineUserDTO
    {
        public string? Id { get; set; }
        public string? ConnectionId { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? PictureUrl { get; set; }
        public bool IsOnline { get; set; }
        public int UnreadMessages { get; set; }
        public string? LatestMessage { get; set; }
        public DateTimeOffset? LatestMessageAt { get; set; }
    }
}
