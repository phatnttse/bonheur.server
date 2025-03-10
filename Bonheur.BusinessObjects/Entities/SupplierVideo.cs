using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.BusinessObjects.Entities
{
    public class SupplierVideo : BaseEntity
    {
        public string? Url { get; set; }
        public string? FileName { get; set; }
        public string? Title { get; set; }
        public string? VideoType { get; set; }
        public int SupplierId { get; set; }
        public virtual Supplier? Supplier { get; set; }
    }
}
