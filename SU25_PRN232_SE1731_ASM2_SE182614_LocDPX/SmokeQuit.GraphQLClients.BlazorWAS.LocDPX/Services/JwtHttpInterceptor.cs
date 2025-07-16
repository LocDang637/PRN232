using System.Net.Http.Headers;

namespace SmokeQuit.GraphQLClients.BlazorWAS.LocDPX.Services
{
    public class JwtHttpInterceptor : DelegatingHandler
    {
        private readonly JwtAuthService _jwtAuthService;

        public JwtHttpInterceptor(JwtAuthService jwtAuthService)
        {
            _jwtAuthService = jwtAuthService;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var authHeader = await _jwtAuthService.GetAuthorizationHeaderAsync();

            if (!string.IsNullOrEmpty(authHeader))
            {
                request.Headers.Authorization = AuthenticationHeaderValue.Parse(authHeader);
            }

            var response = await base.SendAsync(request, cancellationToken);

            // Handle 401 Unauthorized responses
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await _jwtAuthService.LogoutAsync();
            }

            return response;
        }
    }
}