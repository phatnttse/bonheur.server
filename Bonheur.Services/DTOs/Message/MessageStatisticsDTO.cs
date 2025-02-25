using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Services.DTOs.Message
{
    public class MessageStatisticsDTO
    {
        public int TotalMessages { get; set; }
        public int ReadMessages { get; set; }
        public int UnreadMessages { get; set; }
    }

}
