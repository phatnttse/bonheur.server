using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services.DTOs.Supplier
{
    public class SupplierImageDTO
    {
        public int Id { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImageFileName { get; set; }
        public bool IsPrimary { get; set; }
    }
}
