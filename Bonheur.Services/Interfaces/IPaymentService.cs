﻿using Bonheur.BusinessObjects.Models;
using Bonheur.Services.DTOs.Payment;
using Bonheur.Services.DTOs.Payment.PayOs;
using Microsoft.AspNetCore.Mvc;
using Net.payOS.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<ApplicationResponse> CreateSubscriptionPackagePaymentLink(SpPaymentRequestDTO spPaymentRequest);
        Task<PaymentResponse> PayOsTransferHandler(WebhookType body);
        Task<ApplicationResponse> GetPaymentRequestInfo(int orderCode);
    }
}
