﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services.DTOs.Payment.PayOs
{
    public record ConfirmWebhook(
        string webhook_url
    );
}
