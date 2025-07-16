using Microsoft.JSInterop;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using SmokeQuit.GraphQLClients.BlazorWAS.LocDPX.Models;
using SmokeQuit.GraphQLClients.BlazorWAS.LocDPX.GraphQLClients;

namespace SmokeQuit.GraphQLClients.BlazorWAS.LocDPX.Services
{
    public class JwtAuthService
    {
        private readonly GraphQLConsumer _graphQLConsumer;
        private readonly IJSRuntime _jsRuntime;
        private readonly string TOKEN_KEY = "authToken";
        private readonly string USER_KEY = "currentUser";

        private string? _token;
        private SystemUserAccount? _currentUser;
        private readonly JwtSecurityTokenHandler _tokenHandler;

        public JwtAuthService(GraphQLConsumer graphQLConsumer, IJSRuntime jsRuntime)
        {
            _graphQLConsumer = graphQLConsumer;
            _jsRuntime = jsRuntime;
            _tokenHandler = new JwtSecurityTokenHandler();
        }

        public SystemUserAccount? CurrentUser => _currentUser;
        public string? Token => _token;
        public bool IsAuthenticated => _currentUser != null && !string.IsNullOrEmpty(_token) && !IsTokenExpired();

        public event Action? AuthenticationStateChanged;

        public async Task InitializeAsync()
        {
            try
            {
                _token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", TOKEN_KEY);
                var userJson = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", USER_KEY);

                if (!string.IsNullOrEmpty(_token) && !string.IsNullOrEmpty(userJson))
                {
                    if (!IsTokenExpired())
                    {
                        _currentUser = JsonSerializer.Deserialize<SystemUserAccount>(userJson);
                        AuthenticationStateChanged?.Invoke();
                    }
                    else
                    {
                        await LogoutAsync();
                    }
                }
            }
            catch (Exception)
            {
                await LogoutAsync();
            }
        }

        public async Task<LoginResult> LoginAsync(string username, string password)
        {
            try
            {
                // Fix: Use LoginAsync instead of GetUserAccount
                var loginResponse = await _graphQLConsumer.LoginAsync(username, password);

                if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.Token))
                {
                    _token = loginResponse.Token;
                    _currentUser = loginResponse.User;

                    // Store in localStorage
                    await _jsRuntime.InvokeVoidAsync("localStorage.setItem", TOKEN_KEY, _token);
                    await _jsRuntime.InvokeVoidAsync("localStorage.setItem", USER_KEY, JsonSerializer.Serialize(_currentUser));

                    AuthenticationStateChanged?.Invoke();
                    return new LoginResult { Success = true, User = _currentUser };
                }

                return new LoginResult { Success = false, ErrorMessage = "Invalid username or password." };
            }
            catch (Exception ex)
            {
                return new LoginResult { Success = false, ErrorMessage = "Login failed: " + ex.Message };
            }
        }

        public async Task LogoutAsync()
        {
            _token = null;
            _currentUser = null;

            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", TOKEN_KEY);
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", USER_KEY);

            AuthenticationStateChanged?.Invoke();
        }

        public async Task<string?> GetAuthorizationHeaderAsync()
        {
            if (IsAuthenticated)
            {
                return $"Bearer {_token}";
            }
            return null;
        }

        private bool IsTokenExpired()
        {
            if (string.IsNullOrEmpty(_token))
                return true;

            try
            {
                var token = _tokenHandler.ReadJwtToken(_token);
                return token.ValidTo <= DateTime.UtcNow.AddMinutes(-5); // 5 minute buffer
            }
            catch
            {
                return true;
            }
        }

        public ClaimsPrincipal GetClaimsPrincipal()
        {
            if (string.IsNullOrEmpty(_token))
                return new ClaimsPrincipal(new ClaimsIdentity());

            try
            {
                var token = _tokenHandler.ReadJwtToken(_token);
                var identity = new ClaimsIdentity(token.Claims, "jwt");
                return new ClaimsPrincipal(identity);
            }
            catch
            {
                return new ClaimsPrincipal(new ClaimsIdentity());
            }
        }

        public bool HasRole(string role)
        {
            var principal = GetClaimsPrincipal();
            return principal.IsInRole(role);
        }

        public string? GetClaim(string claimType)
        {
            var principal = GetClaimsPrincipal();
            return principal.FindFirst(claimType)?.Value;
        }
    }

    public class LoginResult
    {
        public bool Success { get; set; }
        public SystemUserAccount? User { get; set; }
        public string? ErrorMessage { get; set; }
    }

    //public class LoginResponse
    //{
    //    public string Token { get; set; } = string.Empty;
    //    public SystemUserAccount User { get; set; } = new();
    //}
}
