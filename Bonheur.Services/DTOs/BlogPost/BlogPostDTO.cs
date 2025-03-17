using Bonheur.BusinessObjects.Entities;
using Bonheur.Services.DTOs.BlogTag;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services.DTOs.BlogPost
{
    public class BlogPostDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty; 
        public string Content { get; set; } = string.Empty;
        public string? ThumbnailUrl { get; set; }
        public string? AuthorId { get; set; }
        public ApplicationUser Author { get; set; } = null!;
        public bool IsPublished { get; set; } = false;
        public int CategoryId { get; set; }
        public BlogCategory Category { get; set; } = null!;
        public List<BlogTagDTO> Tags { get; set; } = new();
        public List<BlogComment> Comments { get; set; } = new();
    }
}
