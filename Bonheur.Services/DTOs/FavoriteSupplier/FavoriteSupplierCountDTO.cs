using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services.DTOs.FavoriteSupplier
{
    public class FavoriteSupplierCountDTO
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int FavoriteCount { get; set; }
    }
}
