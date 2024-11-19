using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.BusinessObjects.Entities
{
    public class SupplierImage : BaseEntity
    {
        public string? ImageUrl { get; set; }
        public int SupplierId { get; set; }
        public bool IsPrimary { get; set; }
        public virtual Supplier? Supplier { get; set; }
    }
}
