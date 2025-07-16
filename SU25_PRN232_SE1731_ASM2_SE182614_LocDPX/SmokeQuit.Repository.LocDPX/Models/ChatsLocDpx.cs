using System;
using System.Collections.Generic;

namespace SmokeQuit.Repository.LocDPX.Models;

public partial class ChatsLocDpx
{
    public int ChatsLocDpxid { get; set; }

    public int UserId { get; set; }

    public int CoachId { get; set; }

    public string Message { get; set; } = null!;

    public string SentBy { get; set; } = null!;

    public string MessageType { get; set; } = null!;

    public bool IsRead { get; set; }

    public string? AttachmentUrl { get; set; }

    public DateTime? ResponseTime { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual CoachesLocDpx Coach { get; set; } = null!;

    public virtual SystemUserAccount User { get; set; } = null!;
}
