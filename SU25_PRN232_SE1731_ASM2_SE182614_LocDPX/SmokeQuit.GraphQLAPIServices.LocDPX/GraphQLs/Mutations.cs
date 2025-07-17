using Microsoft.IdentityModel.Tokens;
using SmokeQuit.Repository.LocDPX.Models;
using SmokeQuit.Services.LocDPX;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HotChocolate.Authorization;

namespace SmokeQuit.GraphQLAPIServices.LocDPX.GraphQLs
{
    public class Mutations
    {
        private readonly IServiceProviders _serviceProvider;
        private readonly JwtSettings _jwtSettings;

        public Mutations(IServiceProviders serviceProvider, JwtSettings jwtSettings)
        {
            _serviceProvider = serviceProvider;
            _jwtSettings = jwtSettings;
        }

        #region ChatsLocDpx Mutations - Full CRUD

        [Authorize]
        public async Task<int> CreateChatsLocDpx(ChatsLocDpxInput createChatsLocDpxInput)
        {
            try
            {
                var chat = new ChatsLocDpx
                {
                    UserId = createChatsLocDpxInput.UserId,
                    CoachId = createChatsLocDpxInput.CoachId,
                    Message = createChatsLocDpxInput.Message,
                    SentBy = createChatsLocDpxInput.SentBy,
                    MessageType = createChatsLocDpxInput.MessageType,
                    IsRead = createChatsLocDpxInput.IsRead,
                    AttachmentUrl = createChatsLocDpxInput.AttachmentUrl,
                    ResponseTime = createChatsLocDpxInput.ResponseTime,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await _serviceProvider.ChatsService.CreateAsync(chat);
                return (int)result;
            }
            catch (Exception ex)
            {
                throw new GraphQLException($"Error creating chat: {ex.Message}");
            }
        }

        [Authorize]
        public async Task<int> UpdateChatsLocDpx(ChatsLocDpxUpdateInput updateChatsLocDpxInput)
        {
            try
            {
                var existingChat = await _serviceProvider.ChatsService.GetGetByIdAsync(updateChatsLocDpxInput.ChatsLocDpxid);
                if (existingChat == null)
                    throw new GraphQLException("Chat not found");

                // Update only editable fields
                existingChat.Message = updateChatsLocDpxInput.Message;
                existingChat.MessageType = updateChatsLocDpxInput.MessageType;
                existingChat.SentBy = updateChatsLocDpxInput.SentBy;
                existingChat.IsRead = updateChatsLocDpxInput.IsRead;
                existingChat.AttachmentUrl = updateChatsLocDpxInput.AttachmentUrl;
                existingChat.ResponseTime = updateChatsLocDpxInput.ResponseTime;

                var result = await _serviceProvider.ChatsService.UpdateAsync(existingChat);
                return (int)result;
            }
            catch (Exception ex)
            {
                throw new GraphQLException($"Error updating chat: {ex.Message}");
            }
        }

        [Authorize(Roles = "1")] // Only admin can delete
        public async Task<bool> DeleteChatsLocDpx(int id)
        {
            try
            {
                var result = await _serviceProvider.ChatsService.DeleteAsync(id);
                return (bool)result;
            }
            catch (Exception ex)
            {
                throw new GraphQLException($"Error deleting chat: {ex.Message}");
            }
        }

        #endregion

        #region CoachesLocDpx Mutations - Complete CRUD

        [Authorize]
        public async Task<int> CreateCoachesLocDpx(CoachesLocDpxInput createCoachInput)
        {
            try
            {
                var coach = new CoachesLocDpx
                {
                    FullName = createCoachInput.FullName,
                    Email = createCoachInput.Email,
                    PhoneNumber = createCoachInput.PhoneNumber,
                    Bio = createCoachInput.Bio,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await _serviceProvider.CoachesService.CreateAsync(coach);
                return result;
            }
            catch (Exception ex)
            {
                throw new GraphQLException($"Error creating coach: {ex.Message}");
            }
        }

        [Authorize]
        public async Task<int> UpdateCoachesLocDpx(CoachesLocDpxUpdateInput updateCoachInput)
        {
            try
            {
                var existingCoach = await _serviceProvider.CoachesService.GetByIdAsync(updateCoachInput.CoachesLocDpxid);
                if (existingCoach == null)
                    throw new GraphQLException("Coach not found");

                // Update fields
                existingCoach.FullName = updateCoachInput.FullName;
                existingCoach.Email = updateCoachInput.Email;
                existingCoach.PhoneNumber = updateCoachInput.PhoneNumber;
                existingCoach.Bio = updateCoachInput.Bio;

                var result = await _serviceProvider.CoachesService.UpdateAsync(existingCoach);
                return result;
            }
            catch (Exception ex)
            {
                throw new GraphQLException($"Error updating coach: {ex.Message}");
            }
        }

        [Authorize(Roles = "1")] // Only admin can delete
        public async Task<bool> DeleteCoachesLocDpx(int id)
        {
            try
            {
                var result = await _serviceProvider.CoachesService.DeleteAsync(id);
                return result;
            }
            catch (Exception ex)
            {
                throw new GraphQLException($"Error deleting coach: {ex.Message}");
            }
        }

        #endregion

        #region JWT Authentication Mutations

        public async Task<LoginResponses> Login(string username, string password)
        {
            try
            {
                var user = await _serviceProvider.UserAccountService.GetUserAccount(username, password);

                if (user != null && user.UserAccountId > 0)
                {
                    var token = GenerateJwtToken(user);
                    return new LoginResponses
                    {
                        Token = token,
                        User = user
                    };
                }

                throw new GraphQLException("Invalid username or password");
            }
            catch (Exception ex)
            {
                throw new GraphQLException($"Login failed: {ex.Message}");
            }
        }

        private string GenerateJwtToken(SystemUserAccount user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserAccountId.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim("FullName", user.FullName),
                    new Claim("EmployeeCode", user.EmployeeCode),
                    new Claim(ClaimTypes.Role, user.RoleId.ToString()),
                    new Claim("IsActive", user.IsActive.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(_jwtSettings.ExpirationDays),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        #endregion
    }

    #region Input Types

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

    #endregion

    public class LoginResponses
    {
        public string Token { get; set; } = string.Empty;
        public SystemUserAccount User { get; set; } = new();
    }
}