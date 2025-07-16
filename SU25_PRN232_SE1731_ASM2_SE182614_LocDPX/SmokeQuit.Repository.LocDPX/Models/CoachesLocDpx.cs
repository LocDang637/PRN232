using System;
using System.Collections.Generic;

namespace SmokeQuit.Repository.LocDPX.Models;

public partial class CoachesLocDpx
{
    public int CoachesLocDpxid { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public string? Bio { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<ChatsLocDpx> ChatsLocDpxes { get; set; } = new List<ChatsLocDpx>();
}
