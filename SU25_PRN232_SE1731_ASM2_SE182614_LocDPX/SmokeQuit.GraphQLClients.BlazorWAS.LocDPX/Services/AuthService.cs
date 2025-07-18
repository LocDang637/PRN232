using Microsoft.AspNetCore.Components;
using SmokeQuit.GraphQLClients.BlazorWAS.LocDPX.Models;

namespace SmokeQuit.GraphQLClients.BlazorWAS.LocDPX.Services
{
    public class AuthService
    {
        private readonly GraphQLService _graphQLService;
        private readonly NavigationManager _navigation;

        public event Action<bool>? AuthStateChanged;
        public bool IsAuthenticated { get; private set; }
        public SystemUserAccount? CurrentUser { get; private set; }

        public AuthService(GraphQLService graphQLService, NavigationManager navigation)
        {
            _graphQLService = graphQLService;
            _navigation = navigation;
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            try
            {
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
                    _graphQLService.SetAuthToken(result.Login.Token);
                    CurrentUser = result.Login.User;
                    IsAuthenticated = true;
                    AuthStateChanged?.Invoke(true);
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public void Logout()
        {
            IsAuthenticated = false;
            CurrentUser = null;
            AuthStateChanged?.Invoke(false);
            _navigation.NavigateTo("/");
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