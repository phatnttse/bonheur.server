using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services.DTOs.Account
{
    public class EmailRequestDTO
    { 
        [Required]
        public required string Email { get; set; }

        [Required]
        public required string Token { get; set; }

        public string? Password { get; set; }
    }
}
