using Bonheur.Services.Interfaces;
using Bonheur.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bonheur.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/invoices")]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;

        public InvoiceController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("supplier")]
        [Authorize(Roles = Constants.Roles.SUPPLIER)]
        public async Task<IActionResult> GetInvoicesBySupplierIdAsync()
        {
            return Ok(await _invoiceService.GetInvoicesBySupplierAsync());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("supplier/{id}")]
        [Authorize(Roles = Constants.Roles.SUPPLIER)]
        public async Task<IActionResult> GetInvoiceByIdAsync(int id)
        {
            return Ok(await _invoiceService.GetInvoiceByIdAsync(id));
        }

    }
}
