using Bonheur.BusinessObjects.Models;
using Bonheur.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet("payos/subscription-package")]
        [ProducesResponseType(200, Type = typeof(ApplicationResponse))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> SubscriptionPackagePayment([FromQuery]int subscriptionPackageId)
        {
            return Ok(await _paymentService.subscriptionPackagePayment(subscriptionPackageId));
        }

       
    }
}
