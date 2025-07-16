using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmokeQuit.Repository.LocDPX.ModelExtensions
{
    public class SearchRequest
    {
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 1;
    } 
    public class ClassSearchChatRequest : SearchRequest
    {
        public string? MessageType { get; set; }
        public string? SentBy { get; set; }
        public bool? IsRead { get; set; }
    }
}
