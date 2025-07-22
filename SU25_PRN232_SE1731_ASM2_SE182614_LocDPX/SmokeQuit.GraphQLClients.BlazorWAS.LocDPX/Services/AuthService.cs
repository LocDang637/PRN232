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
        public bool IsAuthenticated { get; private set; }
        public SystemUserAccount? CurrentUser { get; private set; }

        private bool _isInitialized = false;
        private bool _isInitializing = false;

        public AuthService(GraphQLService graphQLService, NavigationManager navigation, IJSRuntime jsRuntime)
        {
            _graphQLService = graphQLService;
            _navigation = navigation;
            _jsRuntime = jsRuntime;
        }

        public async Task InitializeAsync()
        {
            if (_isInitialized || _isInitializing) return;

            _isInitializing = true;

            try
            {
                Console.WriteLine("AuthService: Starting initialization...");

                // Try to restore authentication state from localStorage
                var token = await GetStoredTokenAsync();
                var userJson = await GetStoredUserAsync();

                Console.WriteLine($"AuthService: Retrieved token: {(!string.IsNullOrEmpty(token) ? "Yes" : "No")}");
                Console.WriteLine($"AuthService: Retrieved user: {(!string.IsNullOrEmpty(userJson) ? "Yes" : "No")}");

                if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(userJson))
                {
                    try
                    {
                        var user = JsonSerializer.Deserialize<SystemUserAccount>(userJson);
                        if (user != null)
                        {
                            Console.WriteLine($"AuthService: Restored user: {user.UserName}");

                            // Set authentication state immediately
                            _graphQLService.SetAuthToken(token);
                            CurrentUser = user;
                            IsAuthenticated = true;

                            Console.WriteLine("AuthService: Authentication state restored successfully");
                            AuthStateChanged?.Invoke(true);
                            _isInitialized = true;
                            _isInitializing = false;
                            return;
                        }
                    }
                    catch (JsonException ex)
                    {
                        Console.WriteLine($"AuthService: Invalid stored data: {ex.Message}");
                        await ClearStoredAuthAsync();
                    }
                }

                // If we get here, authentication failed
                Console.WriteLine("AuthService: No valid authentication found");
                IsAuthenticated = false;
                CurrentUser = null;
                AuthStateChanged?.Invoke(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AuthService: Initialization error: {ex.Message}");
                IsAuthenticated = false;
                CurrentUser = null;
                AuthStateChanged?.Invoke(false);
            }
            finally
            {
                _isInitialized = true;
                _isInitializing = false;
            }
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            try
            {
                Console.WriteLine($"AuthService: Attempting login for {username}");

                var query = @"
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
                var result = await _graphQLService.QueryAsync<LoginResponse>(query, variables);

                if (result?.Login != null && !string.IsNullOrEmpty(result.Login.Token))
                {
                    Console.WriteLine($"AuthService: Login successful for {result.Login.User.UserName}");

                    // Store token and user data FIRST
                    await StoreTokenAsync(result.Login.Token);
                    await StoreUserAsync(result.Login.User);

                    Console.WriteLine("AuthService: Token and user stored in localStorage");

                    // Set authentication state
                    _graphQLService.SetAuthToken(result.Login.Token);
                    CurrentUser = result.Login.User;
                    IsAuthenticated = true;

                    Console.WriteLine("AuthService: Authentication state updated");

                    // Notify listeners
                    AuthStateChanged?.Invoke(true);

                    Console.WriteLine("AuthService: Login process completed successfully");
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

                // Clear stored data
                await ClearStoredAuthAsync();

                // Clear in-memory state
                IsAuthenticated = false;
                CurrentUser = null;

                // Clear GraphQL service token
                _graphQLService.ClearAuthToken();

                Console.WriteLine("AuthService: Logout completed");

                // Notify listeners
                AuthStateChanged?.Invoke(false);

                // Navigate to login
                _navigation.NavigateTo("/login", forceLoad: true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AuthService: Logout error: {ex.Message}");
                // Force navigation even if there's an error
                _navigation.NavigateTo("/login", forceLoad: true);
            }
        }

        public void Logout()
        {
            // Synchronous version for compatibility
            _ = LogoutAsync();
        }

        private async Task StoreTokenAsync(string token)
        {
            try
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", TOKEN_KEY, token);
                Console.WriteLine("AuthService: Token stored successfully");
            }
            catch (JSException ex) when (ex.Message.Contains("statically rendered"))
            {
                // Ignore JS errors during prerendering
                Console.WriteLine("AuthService: Skipping token storage during prerendering");
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
                Console.WriteLine("AuthService: User data stored successfully");
            }
            catch (JSException ex) when (ex.Message.Contains("statically rendered"))
            {
                // Ignore JS errors during prerendering
                Console.WriteLine("AuthService: Skipping user storage during prerendering");
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
            catch (JSException ex) when (ex.Message.Contains("statically rendered"))
            {
                // Return null during prerendering
                Console.WriteLine("AuthService: Cannot access localStorage during prerendering");
                return null;
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
            catch (JSException ex) when (ex.Message.Contains("statically rendered"))
            {
                // Return null during prerendering
                Console.WriteLine("AuthService: Cannot access localStorage during prerendering");
                return null;
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
                Console.WriteLine("AuthService: Stored authentication cleared");
            }
            catch (JSException ex) when (ex.Message.Contains("statically rendered"))
            {
                // Ignore JS errors during prerendering
                Console.WriteLine("AuthService: Cannot clear localStorage during prerendering");
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

    public class ValidateTokenResponse
    {
        public bool ValidateToken { get; set; }
    }
}