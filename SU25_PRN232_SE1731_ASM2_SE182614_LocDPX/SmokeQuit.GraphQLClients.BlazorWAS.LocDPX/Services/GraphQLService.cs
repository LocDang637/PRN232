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
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
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

                var response = await _httpClient.PostAsync("/graphql", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"GraphQL Response Status: {response.StatusCode}");
                Console.WriteLine($"GraphQL Response: {responseContent}");

                if (!response.IsSuccessStatusCode)
                {
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