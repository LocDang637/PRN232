using HotChocolate.Authorization;
using Microsoft.IdentityModel.Tokens;
using SmokeQuit.Repository.LocDPX.ModelExtensions;
using SmokeQuit.Repository.LocDPX.Models;
using SmokeQuit.Services.LocDPX;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SmokeQuit.GraphQLAPIServices.LocDPX.GraphQLs
{
    public class Queries
    {
        private readonly IServiceProviders _serviceProvider;
        private readonly JwtSettings _jwtSettings;

        public Queries(IServiceProviders serviceProvider, JwtSettings jwtSettings)
        {
            _serviceProvider = serviceProvider;
            _jwtSettings = jwtSettings;
        }

        #region JWT Authentication Queries

        [Authorize]
        public async Task<SystemUserAccount?> GetCurrentUser([Service] IHttpContextAccessor httpContextAccessor)
        {
            try
            {
                var httpContext = httpContextAccessor.HttpContext;
                var userIdClaim = httpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);

                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    return await _serviceProvider.UserAccountService.GetByIdAsync(userId);
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new GraphQLException($"Error getting current user: {ex.Message}");
            }
        }

        public async Task<bool> ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _jwtSettings.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region ChatsLocDpx Queries - Enhanced CRUD

        [Authorize]
        public async Task<List<ChatsLocDpx>> GetChatsLocDpxes()
        {
            try
            {
                var result = await _serviceProvider.ChatsService.GetAllAsync();
                return result.Items.ToList() ?? new List<ChatsLocDpx>();
            }
            catch (Exception ex)
            {
                throw new GraphQLException($"Error getting chats: {ex.Message}");
            }
        }

        [Authorize]
        public async Task<ChatsLocDpx?> GetChatsLocDpxById(int id)
        {
            try
            {
                var result = await _serviceProvider.ChatsService.GetGetByIdAsync(id);
                return result;
            }
            catch (Exception ex)
            {
                throw new GraphQLException($"Error getting chat by ID: {ex.Message}");
            }
        }

        // ✅ FIXED: Add the missing getChatsWithPaging resolver
        [Authorize]
        public async Task<PaginationResult<ChatsLocDpx>> GetChatsWithPaging(int currentPage = 1, int pageSize = 10)
        {
            try
            {
                var result = await _serviceProvider.ChatsService.GetAllAsync(currentPage, pageSize);
                return result ?? new PaginationResult<ChatsLocDpx>();
            }
            catch (Exception ex)
            {
                throw new GraphQLException($"Error getting chats with pagination: {ex.Message}");
            }
        }

        [Authorize]
        public async Task<PaginationResult<ChatsLocDpx>> SearchChatsWithPaging(ClassSearchChatRequest request)
        {
            try
            {
                var result = await _serviceProvider.ChatsService.SearchAsyncWithPagination(
                    request.MessageType, request.SentBy, request.IsRead, request.CurrentPage, request.PageSize);
                return result ?? new PaginationResult<ChatsLocDpx>();
            }
            catch (Exception ex)
            {
                throw new GraphQLException($"Error searching chats: {ex.Message}");
            }
        }

        // Enhanced search by message content
        [Authorize]
        public async Task<List<ChatsLocDpx>> SearchChatsByMessage(string messageContent)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(messageContent))
                    return new List<ChatsLocDpx>();

                var allChats = await _serviceProvider.ChatsService.GetAllAsync();
                return allChats.Items
                    .Where(c => c.Message != null && c.Message.Contains(messageContent, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new GraphQLException($"Error searching chats by message: {ex.Message}");
            }
        }

        // Search by multiple criteria
        [Authorize]
        public async Task<List<ChatsLocDpx>> SearchChats(string? messageContent, string? messageType, string? sentBy, bool? isRead)
        {
            try
            {
                var result = await _serviceProvider.ChatsService.SearchAsync(messageType, sentBy, isRead);

                // Additional filter by message content if provided
                if (!string.IsNullOrWhiteSpace(messageContent))
                {
                    result = result.Where(c => c.Message != null &&
                        c.Message.Contains(messageContent, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new GraphQLException($"Error searching chats: {ex.Message}");
            }
        }

        #endregion

        #region CoachesLocDpx Queries - Complete CRUD

        [Authorize]
        public async Task<List<CoachesLocDpx>> GetCoachesLocDpxes()
        {
            try
            {
                var result = await _serviceProvider.CoachesService.GetAllAsync();
                return result?.ToList() ?? new List<CoachesLocDpx>();
            }
            catch (Exception ex)
            {
                throw new GraphQLException($"Error getting coaches: {ex.Message}");
            }
        }

        [Authorize]
        public async Task<CoachesLocDpx?> GetCoachesLocDpxById(int id)
        {
            try
            {
                var result = await _serviceProvider.CoachesService.GetByIdAsync(id);
                return result;
            }
            catch (Exception ex)
            {
                throw new GraphQLException($"Error getting coach by ID: {ex.Message}");
            }
        }

        [Authorize]
        public async Task<CoachesLocDpx?> GetCoachByEmail(string email)
        {
            try
            {
                var result = await _serviceProvider.CoachesService.GetByEmailAsync(email);
                return result;
            }
            catch (Exception ex)
            {
                throw new GraphQLException($"Error getting coach by email: {ex.Message}");
            }
        }

        // ✅ FIXED: Add the missing getCoachesWithPaging resolver
        [Authorize]
        public async Task<PaginationResult<CoachesLocDpx>> GetCoachesWithPaging(int currentPage = 1, int pageSize = 10)
        {
            try
            {
                var result = await _serviceProvider.CoachesService.GetAllWithPagingAsync(currentPage, pageSize);
                return result ?? new PaginationResult<CoachesLocDpx>();
            }
            catch (Exception ex)
            {
                throw new GraphQLException($"Error getting coaches with pagination: {ex.Message}");
            }
        }

        [Authorize]
        public async Task<List<CoachesLocDpx>> SearchCoaches(string? fullName, string? email)
        {
            try
            {
                var result = await _serviceProvider.CoachesService.SearchAsync(fullName ?? "", email ?? "");
                return result?.ToList() ?? new List<CoachesLocDpx>();
            }
            catch (Exception ex)
            {
                throw new GraphQLException($"Error searching coaches: {ex.Message}");
            }
        }

        [Authorize]
        public async Task<PaginationResult<CoachesLocDpx>> SearchCoachesWithPaging(string? fullName, string? email, int currentPage = 1, int pageSize = 10)
        {
            try
            {
                var result = await _serviceProvider.CoachesService.SearchWithPagingAsync(
                    fullName ?? "", email ?? "", currentPage, pageSize);
                return result ?? new PaginationResult<CoachesLocDpx>();
            }
            catch (Exception ex)
            {
                throw new GraphQLException($"Error searching coaches with pagination: {ex.Message}");
            }
        }

        #endregion

        #region SystemUserAccount Queries

        public async Task<SystemUserAccount?> GetUserAccount(string username, string password)
        {
            try
            {
                var result = await _serviceProvider.UserAccountService.GetUserAccount(username, password);
                return result;
            }
            catch (Exception ex)
            {
                throw new GraphQLException($"Error getting user account: {ex.Message}");
            }
        }

        [Authorize] // Only admin
        public async Task<List<SystemUserAccount>> GetSystemUserAccounts()
        {
            try
            {
                var result = await _serviceProvider.UserAccountService.GetAllAsync();
                return result.ToList() ?? new List<SystemUserAccount>();
            }
            catch (Exception ex)
            {
                throw new GraphQLException($"Error getting user accounts: {ex.Message}");
            }
        }

        #endregion
    }
}