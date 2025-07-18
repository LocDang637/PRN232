namespace SmokeQuit.GraphQLClients.BlazorWAS.LocDPX.Models
{
    public class ChatsLocDpx
    {
        public int ChatsLocDpxid { get; set; }
        public int UserId { get; set; }
        public int CoachId { get; set; }
        public string Message { get; set; } = string.Empty;
        public string SentBy { get; set; } = string.Empty;
        public string MessageType { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public string? AttachmentUrl { get; set; }
        public DateTime? ResponseTime { get; set; }
        public DateTime? CreatedAt { get; set; }
        public CoachesLocDpx? Coach { get; set; }
    }

    public class ChatsLocDpxInput
    {
        public int UserId { get; set; }
        public int CoachId { get; set; }
        public string Message { get; set; } = string.Empty;
        public string SentBy { get; set; } = string.Empty;
        public string MessageType { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public string? AttachmentUrl { get; set; }
        public DateTime? ResponseTime { get; set; }
    }

    public class ChatsLocDpxUpdateInput
    {
        public int ChatsLocDpxid { get; set; }
        public string Message { get; set; } = string.Empty;
        public string SentBy { get; set; } = string.Empty;
        public string MessageType { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public string? AttachmentUrl { get; set; }
        public DateTime? ResponseTime { get; set; }
    }
}
