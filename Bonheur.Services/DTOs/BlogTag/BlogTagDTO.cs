using Bonheur.Services.DTOs.BlogPost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services.DTOs.BlogTag
{
    public class BlogTagDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public List<BlogPostDTO> BlogPosts { get; set; } = new();
    }
}
