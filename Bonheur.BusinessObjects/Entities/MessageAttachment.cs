using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.BusinessObjects.Entities
{
    public class MessageAttachment : BaseEntity
    {
        public int MessageId { get; set; }
        public virtual Message? Message { get; set; } 

        public string? FileName { get; set; } 
        public string? FilePath { get; set; }
        public string? FileType { get; set; }
    }

}
