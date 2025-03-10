using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.BusinessObjects.Entities
{
    public class BlogComment : BaseEntity
    {
        public string Content { get; set; } = string.Empty;
        public int PostId { get; set; }
        public BlogPost Post { get; set; } = null!;
        public string? UserId { get; set; }
        public ApplicationUser User { get; set; } = null!;
    }
}
