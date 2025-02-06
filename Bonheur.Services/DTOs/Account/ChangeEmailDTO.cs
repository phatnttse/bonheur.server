using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services.DTOs.Account
{
    public class ChangeEmailDTO
    {
        [Required]
        public required string Email { get; set; }
    }
}
