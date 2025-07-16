using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmokeQuit.GraphQLClients.BlazorWAS.LocDPX.Models;

public partial class ChatsLocDpx
{
    [Required]
    public int ChatsLocDpxid { get; set; }
    [Required]
    public int UserId { get; set; }
    [Required]
    public int CoachId { get; set; }
    [Required]
    public string Message { get; set; } = null!;
    [Required]
    public string SentBy { get; set; } = null!;
    [Required]
    public string MessageType { get; set; } = null!;
    [Required]
    public bool IsRead { get; set; }
    public string? AttachmentUrl { get; set; }
    public DateTime? ResponseTime { get; set; }
    public DateTime? CreatedAt { get; set; }
    public virtual CoachesLocDpx Coach { get; set; } = null!;
    public virtual SystemUserAccount User { get; set; } = null!;
}

public partial class ChatsLocDpxesResponse
{
    public List<ChatsLocDpx> chatsLocDpxes { get; set; }
}

public partial class ChatsLocDpxResponse
{
    public ChatsLocDpx chatsLocDpxById { get; set; }
}

public class PaginationResult<T>
{
    public int totalPages { get; set; }
    public int currentPage { get; set; }
    public int pageSize { get; set; }
    public int totalItems { get; set; }
    public List<T> items { get; set; }
}

public partial class ChatsLocDpxesWithPaginationResponse
{
    public PaginationResult<ChatsLocDpx> searchChatsWithPaging { get; set; }
}

// Search Request Models
public class ClassSearchChatsRequestInput
{
    public int? CurrentPage { get; set; } = 1;
    public int? PageSize { get; set; } = 10;
    public string? Message { get; set; }
    public string? SentBy { get; set; }
    public string? MessageType { get; set; }
}

// Mutation Response Models
public partial class CreateChatsLocDpxResponse
{
    public int createChatsLocDpx { get; set; }
}

public partial class UpdateChatsLocDpxResponse
{
    public int updateChatsLocDpx { get; set; }
}

public partial class DeleteChatsLocDpxResponse
{
    public bool deleteChatsLocDpx { get; set; }
}

// JWT Authentication Models
public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public SystemUserAccount User { get; set; } = new();
}

public partial class LoginMutationResponse
{
    public LoginResponse login { get; set; } = new();
}

public partial class ValidateTokenResponse
{
    public bool validateToken { get; set; }
}

public partial class CurrentUserResponse
{
    public SystemUserAccount getCurrentUser { get; set; } = new();
}

// Update existing SystemUserAccountResponse to fix the property name
public partial class SystemUserAccountResponse
{
    public SystemUserAccount getUserAccount { get; set; } = new(); // Fixed: was systemUserAccountById
}

