using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services.DTOs.Account
{
    public class GoogleAccountDTO
    {
        public string? GoogleId { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PictureUrl { get; set; }
        public bool EmailConfirmed { get; set; } = true;
    }
}
