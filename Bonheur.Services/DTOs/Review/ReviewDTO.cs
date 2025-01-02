using Bonheur.BusinessObjects.Entities;
using Bonheur.Services.DTOs.Account;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services.DTOs.Review
{
    public class ReviewDTO
    {
        public string UserId { get; set; }

        public int SupplierId { get; set; }

        public string Content { get; set; }

        public int Rate { get; set; }

        public virtual UserAccountDTO User { get; set; }

    }
}
