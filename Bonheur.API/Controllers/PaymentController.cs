using Azure;
using Bonheur.BusinessObjects.Models;
using Bonheur.Services.DTOs.Payment;
using Bonheur.Services.DTOs.Payment.PayOs;
using Bonheur.Services.Interfaces;
using Bonheur.Utils;
using Microsoft.AspNetCore.Mvc;
using Net.payOS;
using Net.payOS.Types;

namespace Bonheur.API.Controllers
{

    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/payment")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly PayOS _payOS;

        public PaymentController(IPaymentService paymentService, PayOS payOS)
        {
            _paymentService = paymentService;
            _payOS = payOS;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spPaymentRequest"></param>
        /// <returns></returns>
        [HttpPost("subscription-package")]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> SubscriptionPackagePayment([FromBody] SpPaymentRequestDTO spPaymentRequest)
        {
            return Ok(await _paymentService.subscriptionPackagePayment(spPaymentRequest));
        }
    
        /// <summary>
        /// 
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost("payos_transfer_handler")]
        public async Task<IActionResult> PayOsTransferHandler(WebhookType body)
        {           
            return Ok(await _paymentService.payOsTransferHandler(body));              
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost("confirm-webhook")]
        public async Task<IActionResult> ConfirmWebhook(ConfirmWebhook body)
        {
            try
            {
                await _payOS.confirmWebhook(body.webhook_url);
                return Ok(new PaymentResponse(0, "Ok", null));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return Ok(new PaymentResponse(-1, "fail", null));
            }

        }
    }
}
