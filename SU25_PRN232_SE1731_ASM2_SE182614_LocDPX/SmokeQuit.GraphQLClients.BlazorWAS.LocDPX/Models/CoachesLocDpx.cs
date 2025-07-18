namespace SmokeQuit.GraphQLClients.BlazorWAS.LocDPX.Models
{
    public class CoachesLocDpx
    {
        public int CoachesLocDpxid { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Bio { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class CoachesLocDpxInput
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Bio { get; set; }
    }

    public class CoachesLocDpxUpdateInput
    {
        public int CoachesLocDpxid { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Bio { get; set; }
    }
}
