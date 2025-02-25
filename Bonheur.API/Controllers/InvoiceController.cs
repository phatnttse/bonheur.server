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
        public async Task<IActionResult> GetInvoicesBySupplierIdAsync([FromQuery] bool? sortAsc, [FromQuery] string? orderBy, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            return Ok(await _invoiceService.GetInvoicesBySupplierAsync(sortAsc, orderBy, pageNumber, pageSize));
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


        [HttpGet]
        [Authorize(Roles = Constants.Roles.ADMIN)]
        public async Task<IActionResult> GetAllInvoicesAsync()
        {
            return Ok(await _invoiceService.GetAllInvoicesAsync());
        }

    }
}
