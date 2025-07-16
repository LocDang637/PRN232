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

        [Authorize] // Add this attribute to methods that require authentication
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

        // ChatsLocDpx Queries - Full CRUD
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
                return new List<ChatsLocDpx>();
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
                return null;
            }
        }
        [Authorize]
        public async Task<PaginationResult<ChatsLocDpx>> SearchChatsWithPaging(ClassSearchChatRequest request)
        {
            try
            {
                var result = await _serviceProvider.ChatsService.SearchAsyncWithPagination(
                     request.MessageType, request.SentBy,request.IsRead, request.CurrentPage, request.PageSize);
                return result ?? new PaginationResult<ChatsLocDpx>();
            }
            catch (Exception ex)
            {
                return new PaginationResult<ChatsLocDpx>();
            }
        }

        // CoachesLocDpx Queries - Only GetAll
        [Authorize]
        public async Task<List<CoachesLocDpx>> GetCoachesLocDpxes()
        {
            try
            {
                var result = await _serviceProvider.CoachesService.GetAllAsync();
                return result.ToList() ?? new List<CoachesLocDpx>();
            }
            catch (Exception ex)
            {
                return new List<CoachesLocDpx>();
            }
        }

        // SystemUserAccount Queries - Only GetUserAccount (login) and GetAll
        public async Task<SystemUserAccount?> GetUserAccount(string username, string password)
        {
            try
            {
                var result = await _serviceProvider.UserAccountService.GetUserAccount(username, password);
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<SystemUserAccount>> GetSystemUserAccounts()
        {
            try
            {
                var result = await _serviceProvider.UserAccountService.GetAllAsync();
                return result.ToList() ?? new List<SystemUserAccount>();
            }
            catch (Exception ex)
            {
                return new List<SystemUserAccount>();
            }
        }
    }
}