using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services.DTOs.Supplier
{
    public class SupplierVideoDTO
    {
        public int Id { get; set; }
        public string? Url { get; set; }
        public string? FileName { get; set; }
        public string? Title { get; set; }
        public string? VideoType { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }
}
