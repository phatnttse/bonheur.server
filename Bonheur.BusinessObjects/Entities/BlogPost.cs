using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.BusinessObjects.Entities
{
    public class BlogPost :BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty; // URL-friendly title
        public string Content { get; set; } = string.Empty;
        public string? ThumbnailUrl { get; set; } // Ảnh đại diện bài viết
        public string? AuthorId { get; set; }
        public ApplicationUser Author { get; set; } = null!;

        public bool IsPublished { get; set; } = false;

        // Quan hệ với Category và Tags
        public int CategoryId { get; set; }
        public BlogCategory Category { get; set; } = null!;
        public List<BlogTag> Tags { get; set; } = new();

        //Quan hệ với Comment
        public List<BlogComment> Comments { get; set; } = new();
    }
}
