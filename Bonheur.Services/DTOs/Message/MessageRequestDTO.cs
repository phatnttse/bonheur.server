﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services.DTOs.Message
{
    public class MessageRequestDTO
    {
        public string? SenderId { get; set; }
        public string? ReceiverId { get; set; }
        public string? Content { get; set; }
    }
}
