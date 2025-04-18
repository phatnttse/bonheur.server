﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bonheur.BusinessObjects.Entities;

namespace Bonheur.Services.DTOs.SocialNetwork
{
    public class SupplierSocialNetworkDTO
    {
        public int? Id { get; set; }
        public int? SocialNetworkId { get; set; }
        public SocialNetworkDTO? SocialNetwork { get; set; }
        public string? Url { get; set; }
    }
}
