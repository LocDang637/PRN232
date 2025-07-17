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
        public int PageSize { get; set; } = 10;
    }

    public class ClassSearchChatRequest : SearchRequest
    {
        public string? MessageType { get; set; }
        public string? SentBy { get; set; }
        public bool? IsRead { get; set; }
        public string? MessageContent { get; set; } // Added for message content search
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class ClassSearchCoachRequest : SearchRequest
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public DateTime? CreatedAfter { get; set; }
        public DateTime? CreatedBefore { get; set; }
    }

    // Input types for GraphQL
    public class ChatSearchInput
    {
        public string? MessageContent { get; set; }
        public string? MessageType { get; set; }
        public string? SentBy { get; set; }
        public bool? IsRead { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class CoachSearchInput
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public DateTime? CreatedAfter { get; set; }
        public DateTime? CreatedBefore { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
