using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmokeQuit.GraphQLClients.BlazorWAS.LocDPX.Models;

public partial class SystemUserAccount
{
    [Required]
    public int UserAccountId { get; set; }
    [Required]
    public string UserName { get; set; } = null!;
    [Required]
    public string Password { get; set; } = null!;
    [Required]
    public string FullName { get; set; } = null!;
    [Required]
    public string Email { get; set; } = null!;
    [Required]
    public string Phone { get; set; } = null!;
    [Required]
    public string EmployeeCode { get; set; } = null!;
    [Required]
    public int RoleId { get; set; }
    public string? RequestCode { get; set; }
    public DateTime? CreatedDate { get; set; }
    public string? ApplicationCode { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public string? ModifiedBy { get; set; }
    [Required]
    public bool IsActive { get; set; }
    public virtual ICollection<ChatsLocDpx> ChatsLocDpxes { get; set; } = new List<ChatsLocDpx>();
}

public partial class SystemUserAccountsResponse
{
    public List<SystemUserAccount> systemUserAccounts { get; set; }
}

public partial class SystemUserAccountResponse
{
    public SystemUserAccount GetUserAccount { get; set; }
}