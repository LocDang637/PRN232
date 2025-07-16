using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace SmokeQuit.GraphQLClients.BlazorWAS.LocDPX.Services
{
    public class JwtAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly JwtAuthService _jwtAuthService;

        public JwtAuthenticationStateProvider(JwtAuthService jwtAuthService)
        {
            _jwtAuthService = jwtAuthService;
            _jwtAuthService.AuthenticationStateChanged += OnAuthenticationStateChanged;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            await _jwtAuthService.InitializeAsync();

            if (_jwtAuthService.IsAuthenticated)
            {
                var principal = _jwtAuthService.GetClaimsPrincipal();
                return new AuthenticationState(principal);
            }

            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        private void OnAuthenticationStateChanged()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public void Dispose()
        {
            _jwtAuthService.AuthenticationStateChanged -= OnAuthenticationStateChanged;
        }
    }
}