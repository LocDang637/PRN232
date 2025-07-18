namespace SmokeQuit.GraphQLClients.BlazorWAS.LocDPX.Models
{
    public class SystemUserAccount
    {
        public int UserAccountId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string EmployeeCode { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public bool IsActive { get; set; }
    }
}
