using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.BusinessObjects.Entities
{
    public class SupplierSocialNetwork : BaseEntity
    {
        public int SupplierId { get; set; }
        public virtual Supplier? Supplier { get; set; }

        public int SocialNetworkId { get; set; }
        public virtual SocialNetwork? SocialNetwork { get; set; }

        [Required]
        public string? Url { get; set; } 
    }
}
