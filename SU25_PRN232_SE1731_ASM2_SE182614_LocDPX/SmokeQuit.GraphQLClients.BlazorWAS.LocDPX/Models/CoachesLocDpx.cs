using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmokeQuit.GraphQLClients.BlazorWAS.LocDPX.Models;

public partial class CoachesLocDpx
{
    [Required]
    public int CoachesLocDpxid { get; set; }
    [Required]
    public string FullName { get; set; } = null!;
    [Required]
    public string Email { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public string? Bio { get; set; }
    public DateTime? CreatedAt { get; set; }
    public virtual ICollection<ChatsLocDpx> ChatsLocDpxes { get; set; } = new List<ChatsLocDpx>();
}

public partial class CoachesLocDpxesResponse
{
    public List<CoachesLocDpx> coachesLocDpxes { get; set; }
}