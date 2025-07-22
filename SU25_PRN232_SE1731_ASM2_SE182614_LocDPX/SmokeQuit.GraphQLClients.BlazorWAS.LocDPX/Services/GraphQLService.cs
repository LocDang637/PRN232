using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SmokeQuit.GraphQLClients.BlazorWAS.LocDPX.Services
{
    public class GraphQLService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private string? _authToken;

        public GraphQLService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;

            // Debug: Log the base address
            Console.WriteLine($"HttpClient BaseAddress: {_httpClient.BaseAddress}");
        }

        public void SetAuthToken(string token)
        {
            _authToken = token;
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            Console.WriteLine($"Auth token set: {token[..Math.Min(token.Length, 20)]}...");
        }

        public void ClearAuthToken()
        {
            _authToken = null;
            _httpClient.DefaultRequestHeaders.Authorization = null;
            Console.WriteLine("Auth token cleared");
        }

        public async Task<T?> QueryAsync<T>(string query, object? variables = null) where T : class
        {
            try
            {
                // Verify base address is set
                if (_httpClient.BaseAddress == null)
                {
                    throw new InvalidOperationException("HttpClient BaseAddress is not configured. Check Program.cs service registration.");
                }

                var request = new
                {
                    query = query,
                    variables = variables
                };

                var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                Console.WriteLine($"GraphQL Request to: {_httpClient.BaseAddress}graphql");
                Console.WriteLine($"GraphQL Request: {json}");

                // Log authorization header
                if (_httpClient.DefaultRequestHeaders.Authorization != null)
                {
                    Console.WriteLine($"Authorization: {_httpClient.DefaultRequestHeaders.Authorization.Scheme} {_httpClient.DefaultRequestHeaders.Authorization.Parameter?[..20]}...");
                }

                var response = await _httpClient.PostAsync("/graphql", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"GraphQL Response Status: {response.StatusCode}");
                Console.WriteLine($"GraphQL Response: {responseContent}");

                if (!response.IsSuccessStatusCode)
                {
                    // Check if it's an authentication error
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        throw new UnauthorizedAccessException("Authentication required or token expired");
                    }

                    throw new HttpRequestException($"GraphQL request failed with status {response.StatusCode}: {responseContent}");
                }

                var result = JsonSerializer.Deserialize<GraphQLResponse<T>>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (result?.Errors?.Any() == true)
                {
                    var errorMessages = string.Join(", ", result.Errors.Select(e => e.Message));
                    Console.WriteLine($"GraphQL Errors: {errorMessages}");

                    // Check for authentication errors in GraphQL errors
                    if (result.Errors.Any(e => e.Message.Contains("Unauthorized") || e.Message.Contains("authentication")))
                    {
                        throw new UnauthorizedAccessException($"GraphQL Authentication Error: {errorMessages}");
                    }

                    throw new Exception($"GraphQL Error: {errorMessages}");
                }

                return result?.Data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GraphQL Exception: {ex}");
                throw;
            }
        }
    }

    public class GraphQLResponse<T>
    {
        public T? Data { get; set; }
        public GraphQLError[]? Errors { get; set; }
    }

    public class GraphQLError
    {
        public string Message { get; set; } = string.Empty;
        public object? Extensions { get; set; }
    }
}