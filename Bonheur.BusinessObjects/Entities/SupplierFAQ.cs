using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.BusinessObjects.Entities
{
    public class SupplierFAQ : BaseEntity
    {
        public int SupplierId { get; set; }
        public virtual Supplier? Supplier { get; set; }

        [Required]
        public string? Question { get; set; }

        [Required]
        public string? Answer { get; set; }
    }
}
