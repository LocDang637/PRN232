using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SmokeQuit.GraphQLClients.BlazorWAS.LocDPX.Models;
using System.Text.Json;

namespace SmokeQuit.GraphQLClients.BlazorWAS.LocDPX.Services
{
    public class AuthService
    {
        private readonly GraphQLService _graphQLService;
        private readonly NavigationManager _navigation;
        private readonly IJSRuntime _jsRuntime;
        private const string TOKEN_KEY = "smokeQuitToken";
        private const string USER_KEY = "smokeQuitUser";

        public event Action<bool>? AuthStateChanged;
        public bool IsAuthenticated { get; private set; } = false;
        public SystemUserAccount? CurrentUser { get; private set; }

        public AuthService(GraphQLService graphQLService, NavigationManager navigation, IJSRuntime jsRuntime)
        {
            _graphQLService = graphQLService;
            _navigation = navigation;
            _jsRuntime = jsRuntime;
        }

        // NO INITIALIZATION - just return immediately
        public Task InitializeAsync()
        {
            Console.WriteLine("AuthService: InitializeAsync called - returning immediately");
            return Task.CompletedTask;
        }

        // Check authentication state when needed (lazy loading)
        public async Task<bool> CheckAuthenticationAsync()
        {
            try
            {
                Console.WriteLine("AuthService: Checking authentication state...");

                var token = await GetStoredTokenAsync();
                var userJson = await GetStoredUserAsync();

                if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(userJson))
                {
                    try
                    {
                        var user = JsonSerializer.Deserialize<SystemUserAccount>(userJson);
                        if (user != null)
                        {
                            Console.WriteLine($"AuthService: Found stored user: {user.UserName}");

                            _graphQLService.SetAuthToken(token);
                            CurrentUser = user;
                            IsAuthenticated = true;
                            AuthStateChanged?.Invoke(true);
                            return true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"AuthService: Error deserializing user: {ex.Message}");
                        await ClearStoredAuthAsync();
                    }
                }

                Console.WriteLine("AuthService: No valid authentication found");
                IsAuthenticated = false;
                CurrentUser = null;
                AuthStateChanged?.Invoke(false);
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AuthService: Error checking authentication: {ex.Message}");
                IsAuthenticated = false;
                CurrentUser = null;
                return false;
            }
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            try
            {
                Console.WriteLine($"AuthService: Attempting login for {username}");

                var mutation = @"
                    mutation Login($username: String!, $password: String!) {
                        login(username: $username, password: $password) {
                            token
                            user {
                                userAccountId
                                userName
                                fullName
                                email
                                phone
                                employeeCode
                                roleId
                                isActive
                            }
                        }
                    }";

                var variables = new { username, password };
                var result = await _graphQLService.QueryAsync<LoginResponse>(mutation, variables);

                if (result?.Login != null && !string.IsNullOrEmpty(result.Login.Token))
                {
                    Console.WriteLine($"AuthService: Login successful for {result.Login.User.UserName}");

                    // Store token and user data
                    await StoreTokenAsync(result.Login.Token);
                    await StoreUserAsync(result.Login.User);

                    // Set authentication state
                    _graphQLService.SetAuthToken(result.Login.Token);
                    CurrentUser = result.Login.User;
                    IsAuthenticated = true;

                    // Notify listeners
                    AuthStateChanged?.Invoke(true);

                    return true;
                }

                Console.WriteLine("AuthService: Login failed - invalid response");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AuthService: Login error: {ex.Message}");
                return false;
            }
        }

        public async Task LogoutAsync()
        {
            try
            {
                Console.WriteLine("AuthService: Starting logout...");

                await ClearStoredAuthAsync();

                IsAuthenticated = false;
                CurrentUser = null;
                _graphQLService.ClearAuthToken();

                AuthStateChanged?.Invoke(false);

                Console.WriteLine("AuthService: Logout completed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AuthService: Logout error: {ex.Message}");
            }
        }

        public void Logout()
        {
            _ = LogoutAsync();
        }

        private async Task StoreTokenAsync(string token)
        {
            try
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", TOKEN_KEY, token);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AuthService: Error storing token: {ex.Message}");
            }
        }

        private async Task StoreUserAsync(SystemUserAccount user)
        {
            try
            {
                var userJson = JsonSerializer.Serialize(user);
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", USER_KEY, userJson);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AuthService: Error storing user: {ex.Message}");
            }
        }

        private async Task<string?> GetStoredTokenAsync()
        {
            try
            {
                return await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", TOKEN_KEY);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AuthService: Error getting stored token: {ex.Message}");
                return null;
            }
        }

        private async Task<string?> GetStoredUserAsync()
        {
            try
            {
                return await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", USER_KEY);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AuthService: Error getting stored user: {ex.Message}");
                return null;
            }
        }

        private async Task ClearStoredAuthAsync()
        {
            try
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", TOKEN_KEY);
                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", USER_KEY);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AuthService: Error clearing stored auth: {ex.Message}");
            }
        }
    }

    public class LoginResponse
    {
        public LoginData Login { get; set; } = new();
    }

    public class LoginData
    {
        public string Token { get; set; } = string.Empty;
        public SystemUserAccount User { get; set; } = new();
    }
}