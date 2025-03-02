using Bonheur.Services.Interfaces;
using Bonheur.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bonheur.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/messages")]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpGet("supplier/statistics")]
        [Authorize(Roles = Constants.Roles.SUPPLIER)]
        public IActionResult GetSupplierMessageStatistics()
        {
            return Ok(_messageService.GetSupplierMessageStatistics());
        }

        [HttpGet("user/unread/count")]
        [Authorize(Roles = Constants.Roles.USER)]
        public async Task<IActionResult> GetUnreadMessagesCountByUser()
        {
            return Ok(await _messageService.GetUnreadMessagesCountByUser());
        }

        [HttpPost("attachment")]
        [Authorize(Roles = Constants.Roles.USER + "," + Constants.Roles.SUPPLIER + "," + Constants.Roles.ADMIN)]
        public async Task<IActionResult> UploadAttachmentFile([FromForm] List<IFormFile> files)
        {
            return Ok(await _messageService.UploadAttachmentFile(files));
        }
    }
}
