using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.BusinessObjects.Entities
{
    public class FavoriteSupplier : BaseEntity
    {
        [ForeignKey("UserId")]
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }
        
        [ForeignKey("SupplierId")]
        public int SupplierId { get; set; }
        public virtual Supplier? Supplier { get; set; }  
    }
}
