using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services.DTOs.SocialNetwork
{
    public class SocialNetworkDTO
    {
        public string? Name { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImageFileName { get; set; }
    }
}
