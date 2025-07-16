using Microsoft.IdentityModel.Tokens;
using SmokeQuit.Repository.LocDPX.Models;
using SmokeQuit.Services.LocDPX;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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

        // ChatsLocDpx Mutations - Full CRUD
        public async Task<int> CreateChatsLocDpx(ChatsLocDpx createChatsLocDpxInput)
        {
            try
            {
                var result = await _serviceProvider.ChatsService.CreateAsync(createChatsLocDpxInput);
                return (int)result;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<int> UpdateChatsLocDpx(ChatsLocDpx updateChatsLocDpxInput)
        {
            try
            {
                var result = await _serviceProvider.ChatsService.UpdateAsync(updateChatsLocDpxInput);
                return (int)result;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<bool> DeleteChatsLocDpx(int id)
        {
            try
            {
                var result = await _serviceProvider.ChatsService.DeleteAsync(id);
                return (bool)result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

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

    public class LoginResponses
    {
        public string Token { get; set; } = string.Empty;
        public SystemUserAccount User { get; set; } = new();
    }
}