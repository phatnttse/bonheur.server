using Azure;
using Bonheur.BusinessObjects.Models;
using Bonheur.Services.DTOs.Payment.PayOs;
using Bonheur.Services.Interfaces;
using Bonheur.Utils;
using Microsoft.AspNetCore.Mvc;
using Net.payOS.Types;

namespace Bonheur.API.Controllers
{

    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/payment")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscriptionPackageId"></param>
        /// <returns></returns>
        [HttpGet("subscription-package")]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> SubscriptionPackagePayment([FromQuery]int spId)
        {
            return Ok(await _paymentService.subscriptionPackagePayment(spId));
        }
    
        [HttpPost("payos_transfer_handler")]
        public IActionResult ConfirmWebhook(WebhookType body)
        {
            try
            {
                _paymentService.payOsTransferHandler(body);
                return Ok(new PaymentResponse(0, "Ok", null));
            }
            catch (ApiException ex)
            {
                return Ok(new PaymentResponse(-1, ex.Message, null));
            }
            catch (Exception ex)
            {
                return Ok(new PaymentResponse(-1, ex.Message, null));
            }
        }
    }
}
