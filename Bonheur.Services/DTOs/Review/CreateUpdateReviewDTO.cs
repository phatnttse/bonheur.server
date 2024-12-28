using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services.DTOs.Review
{
    public class CreateUpdateReviewDTO
    {
        public string UserId { get; set; }

        public int SupplierId { get; set; }

        public string Content { get; set; }

        public int Rate { get; set; }
    }
}
