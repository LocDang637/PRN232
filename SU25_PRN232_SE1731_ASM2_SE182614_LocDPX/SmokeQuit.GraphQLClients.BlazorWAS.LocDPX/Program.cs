using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SmokeQuit.GraphQLClients.BlazorWAS.LocDPX;
using SmokeQuit.GraphQLClients.BlazorWAS.LocDPX.Components;
using SmokeQuit.GraphQLClients.BlazorWAS.LocDPX.GraphQLClients;
using SmokeQuit.GraphQLClients.BlazorWAS.LocDPX.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// JWT Authentication Services
builder.Services.AddScoped<JwtAuthService>();
builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();
builder.Services.AddAuthorizationCore();

// HTTP Client with JWT Interceptor
builder.Services.AddScoped<JwtHttpInterceptor>();
builder.Services.AddScoped(sp =>
{
    var jwtInterceptor = sp.GetRequiredService<JwtHttpInterceptor>();
    jwtInterceptor.InnerHandler = new HttpClientHandler();

    return new HttpClient(jwtInterceptor)
    {
        BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
    };
});

// GraphQL Client with JWT support
builder.Services.AddScoped<IGraphQLClient>(sp =>
{
    var httpClient = sp.GetRequiredService<HttpClient>();
    return new GraphQLHttpClient(
        new GraphQLHttpClientOptions
        {
            EndPoint = new Uri(builder.Configuration["GraphQLURI"])
        },
        new NewtonsoftJsonSerializer(),
        httpClient
    );
});

builder.Services.AddScoped<GraphQLConsumer>();

await builder.Build().RunAsync();