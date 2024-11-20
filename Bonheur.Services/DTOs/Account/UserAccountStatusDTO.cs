using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services.DTOs.Account
{
    public class UserAccountStatusDTO
    {
        public DateTimeOffset? LockoutEnd { get; set; }
        public bool? IsEnabled { get; set; }
    }
}
