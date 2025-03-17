using Bonheur.BusinessObjects.Entities;
using Bonheur.Services.DTOs.Notification;
using Bonheur.Services.Interfaces;
using Bonheur.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bonheur.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/notifications")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = Constants.Roles.ADMIN)]
        public async Task<IActionResult> CreateNotification([FromBody] CreateNotificationDTO request)
        {
            return Ok(await _notificationService.CreateNotification(request));
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetNotificationsByAccountAsync([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            return Ok(await _notificationService.GetNotificationsByAccountAsync(pageNumber, pageSize));
        }

    }
}
