using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services.DTOs.PaginationParams
{
    public class PaginationParams
    {
        private readonly int _maxPageSize;

        public PaginationParams(IConfiguration configuration)
        {
            _maxPageSize = configuration.GetValue<int>("PaginationSettings:MaxPageSize", 100); 
        }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int GetPageSize => (PageSize > _maxPageSize) ? _maxPageSize : PageSize;
    }
}

