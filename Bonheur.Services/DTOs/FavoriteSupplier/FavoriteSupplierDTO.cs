using Bonheur.BusinessObjects.Entities;
using Bonheur.Services.DTOs.Account;
using Bonheur.Services.DTOs.Supplier;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services.DTOs.FavoriteSupplier
{
    public class FavoriteSupplierDTO
    {
        [Required]
        public string? UserId { get; set; }

        [Required]
        public int SupplierId { get; set; }
        public virtual SupplierDTO?  Supplier { get; set; }
    }
}
